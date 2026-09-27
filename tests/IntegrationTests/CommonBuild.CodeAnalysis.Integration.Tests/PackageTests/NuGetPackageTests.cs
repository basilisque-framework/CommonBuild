namespace CommonBuild.CodeAnalysis.Integration.Tests.PackageTests;

internal class NuGetPackageTests
{
	private const string _packageId = "Consumer.CodeAnalysis";

	[Test]
	public async Task Ensure_Package_Contains_AnalyzerAssembly_InAnalyzersDotnetCs()
	{
		var unpackedPackagePath = getUnpackedPackagePath();
		var packageVersion = getPackageVersion(unpackedPackagePath);
		var expectedAnalyzerDllRelativePath = System.IO.Path.Combine("analyzers", "dotnet", "cs", $"Consumer.CodeAnalysis-{packageVersion}.dll");
		var analyzerDllPath = System.IO.Path.Combine(unpackedPackagePath, expectedAnalyzerDllRelativePath);

		await Assert.That(System.IO.File.Exists(analyzerDllPath)).IsTrue();
	}

	[Test]
	public async Task Ensure_Packaged_AssemblyName_Contains_PackageVersionSuffix()
	{
		var unpackedPackagePath = getUnpackedPackagePath();
		var packageVersion = getPackageVersion(unpackedPackagePath);
		var analyzerDirectory = System.IO.Path.Combine(unpackedPackagePath, "analyzers", "dotnet", "cs");
		var analyzerDllPath = System.IO.Directory.EnumerateFiles(analyzerDirectory, "*.dll").Single();
		var assemblyName = System.Reflection.AssemblyName.GetAssemblyName(analyzerDllPath).Name;

		await Assert.That(assemblyName).IsEqualTo($"{_packageId}-{packageVersion}");
	}

	[Test]
	public async Task Ensure_Package_DoesNotContain_Assembly_InLibDirectory()
	{
		var unpackedPackagePath = getUnpackedPackagePath();
		var libDirectoryPath = System.IO.Path.Combine(unpackedPackagePath, "lib");

		var libDllCount = System.IO.Directory.Exists(libDirectoryPath)
			? System.IO.Directory.EnumerateFiles(libDirectoryPath, "*.dll", System.IO.SearchOption.AllDirectories).Count()
			: 0;

		await Assert.That(libDllCount).IsEqualTo(0);
	}

	[Test]
	public async Task Ensure_Package_Contains_Only_One_Dll_AndItIsTheAnalyzerAssembly()
	{
		var unpackedPackagePath = getUnpackedPackagePath();
		var packageVersion = getPackageVersion(unpackedPackagePath);
		var expectedAnalyzerDllRelativePath = System.IO.Path.Combine("analyzers", "dotnet", "cs", $"Consumer.CodeAnalysis-{packageVersion}.dll");
		var dllEntries = System.IO.Directory
			.EnumerateFiles(unpackedPackagePath, "*.dll", System.IO.SearchOption.AllDirectories)
			.Select(path => normalizePackagePath(path.Substring(unpackedPackagePath.Length).TrimStart(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)))
			.ToList();
		var expectedRelativePath = normalizePackagePath(expectedAnalyzerDllRelativePath);

		await Assert.That(dllEntries.Count).IsEqualTo(1);
		await Assert.That(dllEntries[0]).IsEqualTo(expectedRelativePath);
	}

	[Test]
	public async Task Ensure_Nuspec_Contains_ExpectedId_AndDependencies()
	{
		var unpackedPackagePath = getUnpackedPackagePath();
		var nuspecPath = System.IO.Path.Combine(unpackedPackagePath, $"{_packageId}.nuspec");

		await Assert.That(System.IO.File.Exists(nuspecPath)).IsTrue();

		var nuspec = System.Xml.Linq.XDocument.Load(nuspecPath);
		var xmlNamespace = nuspec.Root?.Name.Namespace ?? throw new Exception("nuspec root element missing.");
		var metadata = nuspec.Root?.Element(xmlNamespace + "metadata") ?? throw new Exception("nuspec metadata element missing.");
		var id = metadata.Element(xmlNamespace + "id")?.Value;

		await Assert.That(id).IsEqualTo(_packageId);

		var dependencyIds = metadata
			.Descendants(xmlNamespace + "dependency")
			.Select(element => element.Attribute("id")?.Value)
			.Where(value => !string.IsNullOrWhiteSpace(value))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		await Assert.That(dependencyIds.Contains("Consumer.Shared")).IsTrue();
		await Assert.That(dependencyIds.Contains("Basilisque.CommonBuild")).IsTrue();
		await Assert.That(dependencyIds.Contains("Microsoft.CodeAnalysis")).IsTrue();
	}

	[Test]
	public async Task Ensure_Nuspec_Title_IsSetTo_UnversionedAssemblyName()
	{
		var unpackedPackagePath = getUnpackedPackagePath();
		var nuspecPath = System.IO.Path.Combine(unpackedPackagePath, $"{_packageId}.nuspec");
		var nuspec = System.Xml.Linq.XDocument.Load(nuspecPath);
		var xmlNamespace = nuspec.Root?.Name.Namespace ?? throw new Exception("nuspec root element missing.");
		var metadata = nuspec.Root?.Element(xmlNamespace + "metadata") ?? throw new Exception("nuspec metadata element missing.");
		var title = metadata.Element(xmlNamespace + "title")?.Value;

		await Assert.That(title).IsEqualTo(_packageId);
	}

	[Test]
	public async Task Ensure_Nuspec_Contains_Repository_Metadata()
	{
		var unpackedPackagePath = getUnpackedPackagePath();
		var nuspecPath = System.IO.Path.Combine(unpackedPackagePath, $"{_packageId}.nuspec");

		await Assert.That(System.IO.File.Exists(nuspecPath)).IsTrue();

		var nuspec = System.Xml.Linq.XDocument.Load(nuspecPath);
		var xmlNamespace = nuspec.Root?.Name.Namespace ?? throw new Exception("nuspec root element missing.");
		var metadata = nuspec.Root?.Element(xmlNamespace + "metadata") ?? throw new Exception("nuspec metadata element missing.");
		var repositoryElement = metadata.Element(xmlNamespace + "repository");

		await Assert.That(repositoryElement).IsNotNull();

		var repositoryType = repositoryElement!.Attribute("type")?.Value;
		var repositoryBranch = repositoryElement.Attribute("branch")?.Value;
		var repositoryCommit = repositoryElement.Attribute("commit")?.Value;

		await Assert.That(repositoryType).IsEqualTo("git");
		await Assert.That(repositoryBranch).IsNotNull().And.IsNotEmpty();
		await Assert.That(repositoryCommit).IsNotNull().And.IsNotEmpty();

		var isHexCommit = repositoryCommit!.Length == 40 && repositoryCommit.All(character =>
			(character >= '0' && character <= '9')
			|| (character >= 'a' && character <= 'f')
			|| (character >= 'A' && character <= 'F'));

		await Assert.That(isHexCommit).IsTrue();
	}

	[Test]
	public async Task Ensure_Package_DoesNotContain_BuildAssetsDirectories()
	{
		var unpackedPackagePath = getUnpackedPackagePath();
		var packageEntries = System.IO.Directory
			.EnumerateFiles(unpackedPackagePath, "*", System.IO.SearchOption.AllDirectories)
			.Select(path => normalizePackagePath(path.Substring(unpackedPackagePath.Length).TrimStart(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)))
			.ToList();

		var hasBuildAssets = packageEntries.Any(entry =>
			entry.StartsWith("build/", StringComparison.OrdinalIgnoreCase)
			|| entry.StartsWith("buildMultitargeting/", StringComparison.OrdinalIgnoreCase)
			|| entry.EndsWith(".props", StringComparison.OrdinalIgnoreCase)
			|| entry.EndsWith(".targets", StringComparison.OrdinalIgnoreCase));

		await Assert.That(hasBuildAssets).IsFalse();
	}

	private static string getUnpackedPackagePath()
	{
		CommonBuild.Integration.TestSupport.TestSetup.UnpackNuGetPackages.EnsureUnpackedPackagesAvailable();

		var unpackedPackagePath = System.IO.Directory
			.EnumerateDirectories(CommonBuild.Integration.TestSupport.TestSetup.UnpackNuGetPackages.UnpackedPackagesPath, $"{_packageId}.*", System.IO.SearchOption.TopDirectoryOnly)
			.OrderByDescending(path => path)
			.FirstOrDefault();

		if (string.IsNullOrWhiteSpace(unpackedPackagePath))
			throw new System.IO.DirectoryNotFoundException($"No unpacked package directory found for package '{_packageId}'.");

		return unpackedPackagePath;
	}

	private static string normalizePackagePath(string packageRelativePath)
	{
		return packageRelativePath.Replace('\\', '/');
	}

	private static string getPackageVersion(string unpackedPackagePath)
	{
		var nuspecPath = System.IO.Path.Combine(unpackedPackagePath, $"{_packageId}.nuspec");
		var nuspec = System.Xml.Linq.XDocument.Load(nuspecPath);
		var xmlNamespace = nuspec.Root?.Name.Namespace ?? throw new Exception("nuspec root element missing.");
		var metadata = nuspec.Root?.Element(xmlNamespace + "metadata") ?? throw new Exception("nuspec metadata element missing.");
		var packageVersion = metadata.Element(xmlNamespace + "version")?.Value;

		if (string.IsNullOrWhiteSpace(packageVersion))
			throw new Exception("Package version is missing in nuspec metadata.");

		return packageVersion;
	}
}
