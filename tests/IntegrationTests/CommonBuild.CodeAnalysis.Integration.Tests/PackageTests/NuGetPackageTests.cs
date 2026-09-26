namespace CommonBuild.CodeAnalysis.Integration.Tests.PackageTests;

internal class NuGetPackageTests
{
	private const string _packageId = "Consumer.CodeAnalysis";
	private static readonly string _expectedAnalyzerDllRelativePath = System.IO.Path.Combine("analyzers", "dotnet", "cs", "Consumer.CodeAnalysis.dll");

	[Test]
	public async Task Ensure_Package_Contains_AnalyzerAssembly_InAnalyzersDotnetCs()
	{
		var unpackedPackagePath = getUnpackedPackagePath();
		var analyzerDllPath = System.IO.Path.Combine(unpackedPackagePath, _expectedAnalyzerDllRelativePath);

		await Assert.That(System.IO.File.Exists(analyzerDllPath)).IsTrue();
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
}
