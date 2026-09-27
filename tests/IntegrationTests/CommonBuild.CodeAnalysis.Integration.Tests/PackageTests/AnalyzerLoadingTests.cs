using CommonBuild.Integration.TestSupport.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Xml.Linq;

namespace CommonBuild.CodeAnalysis.Integration.Tests.PackageTests;

internal class AnalyzerLoadingTests
{
    private const string _packageId = "PackageLoad.CodeAnalysis";

    [Test]
    public async Task Ensure_Installed_Analyzer_Package_Reports_Diagnostic_In_Consumer_Build()
    {
        using var workspace = new PackageWorkspace();
        await workspace.PackAnalyzer(1);

        var consumerDirectory = Path.Combine(workspace.Root, "Consumer");
        Directory.CreateDirectory(consumerDirectory);
        var consumerProject = Path.Combine(consumerDirectory, "Consumer.csproj");
        new XDocument(new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
            new XElement("PropertyGroup",
                new XElement("TargetFramework", $"net{Environment.Version.Major}.0"),
                new XElement("TreatWarningsAsErrors", true),
                new XElement("WarningsNotAsErrors", "CBTEST001")),
            new XElement("ItemGroup",
                new XElement("PackageReference", new XAttribute("Include", _packageId), new XAttribute("Version", "7.3.1")))))
            .Save(consumerProject);
        await File.WriteAllTextAsync(Path.Combine(consumerDirectory, "Consumer.cs"), "public sealed class Consumer { }");

        var result = await workspace.RunDotnet("build", consumerProject, "-c", "Release", "--disable-build-servers", "-p:UseSharedCompilation=false");

        await Assert.That(result.ExitCode).IsEqualTo(0).Because(result.Output);
        await Assert.That(result.Output).Contains("CBTEST001");
        await Assert.That(result.Output).Contains($"{_packageId}-7.3.1");

        var assets = System.Text.Json.JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(consumerDirectory, "obj", "project.assets.json")));
        using (assets)
        {
            var packageFiles = assets.RootElement.GetProperty("libraries").GetProperty($"{_packageId}/7.3.1").GetProperty("files")
                .EnumerateArray().Select(entry => entry.GetString() ?? throw new InvalidDataException("Package file path is null.")).ToArray();
            await Assert.That(packageFiles).Contains("lib/netstandard2.0/_._");
            await Assert.That(packageFiles).Contains($"analyzers/dotnet/cs/{_packageId}-7.3.1.dll");
        }
    }

    [Test]
    public async Task Ensure_Two_Packaged_Analyzer_Versions_Execute_In_One_LoadContext()
    {
        using var workspace = new PackageWorkspace();
        var firstPath = await workspace.PackAnalyzer(1);
        var secondPath = await workspace.PackAnalyzer(2);
        var loader = new PackageAnalyzerLoader();
        try
        {
            var failures = new List<string>();
            var firstReference = new AnalyzerFileReference(firstPath, loader);
            var secondReference = new AnalyzerFileReference(secondPath, loader);
            firstReference.AnalyzerLoadFailed += (_, failure) => failures.Add(failure.Message);
            secondReference.AnalyzerLoadFailed += (_, failure) => failures.Add(failure.Message);
            var firstAnalyzers = firstReference.GetAnalyzers(LanguageNames.CSharp);
            var secondAnalyzers = secondReference.GetAnalyzers(LanguageNames.CSharp);

            await Assert.That(failures.Count).IsEqualTo(0).Because(string.Join(Environment.NewLine, failures));
            await Assert.That(firstAnalyzers.Length).IsEqualTo(1);
            await Assert.That(secondAnalyzers.Length).IsEqualTo(1);
            var firstAssembly = firstAnalyzers[0].GetType().Assembly;
            var secondAssembly = secondAnalyzers[0].GetType().Assembly;
            await Assert.That(firstAssembly.GetName().Name).IsEqualTo($"{_packageId}-7.3.1");
            await Assert.That(secondAssembly.GetName().Name).IsEqualTo($"{_packageId}-7.3.2");
            await Assert.That(ReferenceEquals(firstAssembly, secondAssembly)).IsFalse();
            await Assert.That(ReferenceEquals(AssemblyLoadContext.GetLoadContext(firstAssembly), loader)).IsTrue();
            await Assert.That(ReferenceEquals(AssemblyLoadContext.GetLoadContext(secondAssembly), loader)).IsTrue();

            var compilation = CSharpCompilation.Create("AnalyzerConsumer",
                new[] { CSharpSyntaxTree.ParseText("public sealed class Consumer { }") },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            var diagnostics = await compilation.WithAnalyzers(firstAnalyzers.AddRange(secondAnalyzers)).GetAnalyzerDiagnosticsAsync();

            await Assert.That(diagnostics.Length).IsEqualTo(2).Because(string.Join(Environment.NewLine, diagnostics));
            await Assert.That(diagnostics.All(diagnostic => diagnostic.Id == "CBTEST001")).IsTrue();
            await Assert.That(diagnostics.Select(diagnostic => diagnostic.GetMessage()).OrderBy(message => message).ToArray())
                .IsEquivalentTo(new[] { $"Analyzer '{_packageId}-7.3.1' executed", $"Analyzer '{_packageId}-7.3.2' executed" });
        }
        finally
        {
            loader.Unload();
        }
    }

    private sealed class PackageAnalyzerLoader : AssemblyLoadContext, IAnalyzerAssemblyLoader
    {
        public PackageAnalyzerLoader() : base(isCollectible: true) { }

        public void AddDependencyLocation(string fullPath) { }

        public Assembly LoadFromPath(string fullPath)
        {
            using var stream = File.OpenRead(fullPath);
            return LoadFromStream(stream);
        }

        protected override Assembly? Load(AssemblyName assemblyName) => null;
    }

    private sealed class PackageWorkspace : IDisposable
    {
        private readonly string _commonBuildVersion;
        private readonly string _roslynVersion;

        public string Root { get; } = Path.Combine(Path.GetTempPath(), "CommonBuild.AnalyzerTests", Guid.NewGuid().ToString("N"));

        public PackageWorkspace()
        {
            Directory.CreateDirectory(Root);
            var commonBuildPath = AssemblyMetadataUtils.CommonBuildNuGetPackagePath;
            _commonBuildVersion = readPackageVersion(Path.Combine(commonBuildPath, "basilisque.commonbuild.nuspec"));
            var roslynPath = typeof(AnalyzerLoadingTests).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .Single(attribute => attribute.Key == "RoslynPackagePath").Value!;
            _roslynVersion = readPackageVersion(Path.Combine(roslynPath, "microsoft.codeanalysis.csharp.nuspec"));
            Directory.CreateDirectory(Path.Combine(Root, "feed"));
            new XDocument(new XElement("configuration",
                new XElement("packageSources",
                    new XElement("clear"),
                    new XElement("add", new XAttribute("key", "fixtures"), new XAttribute("value", Path.Combine(Root, "feed"))),
                    new XElement("add", new XAttribute("key", "commonbuild"), new XAttribute("value", commonBuildPath)),
                    new XElement("add", new XAttribute("key", "nuget.org"), new XAttribute("value", "https://api.nuget.org/v3/index.json"))),
                new XElement("config", new XElement("add", new XAttribute("key", "globalPackagesFolder"), new XAttribute("value", Path.Combine(Root, "packages"))))))
                .Save(Path.Combine(Root, "NuGet.Config"));
        }

        public async Task<string> PackAnalyzer(int patchVersion)
        {
            var directory = Path.Combine(Root, $"version-{patchVersion}");
            Directory.CreateDirectory(directory);
            var projectPath = Path.Combine(directory, $"{_packageId}.csproj");
            new XDocument(new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("TargetFramework", "netstandard2.0"),
                    new XElement("BAS_CB_Use_GitVersion", false),
                    new XElement("BAS_CB_BuildType", "Release"),
                    new XElement("BAS_CB_VersionMajor", 7),
                    new XElement("BAS_CB_VersionMinor", 3),
                    new XElement("BAS_CB_VersionBuild", patchVersion),
                    new XElement("EnforceExtendedAnalyzerRules", true),
                    new XElement("NoWarn", "RS2008")),
                new XElement("ItemGroup",
                    new XElement("PackageReference", new XAttribute("Include", "Basilisque.CommonBuild"), new XAttribute("Version", _commonBuildVersion), new XAttribute("PrivateAssets", "all")),
                    new XElement("PackageReference", new XAttribute("Include", "Microsoft.CodeAnalysis.CSharp"), new XAttribute("Version", _roslynVersion), new XAttribute("PrivateAssets", "all")))))
                .Save(projectPath);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "TestAssets", "FixtureDiagnosticAnalyzer.cs"), Path.Combine(directory, "FixtureDiagnosticAnalyzer.cs"));

            var result = await RunDotnet("pack", projectPath, "-c", "Release", "-o", Path.Combine(Root, "feed"), "--disable-build-servers", "-p:UseSharedCompilation=false", "-p:TreatWarningsAsErrors=true");
            await Assert.That(result.ExitCode).IsEqualTo(0).Because(result.Output);

            var unpackedPath = Path.Combine(directory, "unpacked");
            ZipFile.ExtractToDirectory(Path.Combine(Root, "feed", $"{_packageId}.7.3.{patchVersion}.nupkg"), unpackedPath);
            return Path.Combine(unpackedPath, "analyzers", "dotnet", "cs", $"{_packageId}-7.3.{patchVersion}.dll");
        }

        public async Task<(int ExitCode, string Output)> RunDotnet(params string[] arguments)
        {
            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = Root,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            foreach (var argument in arguments)
                startInfo.ArgumentList.Add(argument);
            startInfo.Environment["DOTNET_CLI_UI_LANGUAGE"] = "en";
            startInfo.Environment["NUGET_PACKAGES"] = Path.Combine(Root, "packages");
            using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Could not start dotnet.");
            var standardOutput = process.StandardOutput.ReadToEndAsync();
            var standardError = process.StandardError.ReadToEndAsync();
            using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(3));
            try
            {
                await process.WaitForExitAsync(timeout.Token);
            }
            catch (OperationCanceledException)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
                throw new TimeoutException($"dotnet {string.Join(" ", arguments)} timed out.\n{await standardOutput}\n{await standardError}");
            }
            return (process.ExitCode, $"{await standardOutput}\n{await standardError}");
        }

        private static string readPackageVersion(string nuspecPath)
        {
            var document = XDocument.Load(nuspecPath);
            var xmlNamespace = document.Root!.Name.Namespace;
            return document.Root.Element(xmlNamespace + "metadata")!.Element(xmlNamespace + "version")!.Value;
        }

        public void Dispose() => Directory.Delete(Root, recursive: true);
    }
}