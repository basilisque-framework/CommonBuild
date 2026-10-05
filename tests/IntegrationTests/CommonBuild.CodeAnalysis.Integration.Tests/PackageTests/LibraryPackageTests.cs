using Consumer.CodeAnalysis.Library;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using CommonBuild.Integration.TestSupport.Utils;

namespace CommonBuild.CodeAnalysis.Integration.Tests.PackageTests;

internal class LibraryPackageTests
{
    private static readonly Assembly _assembly = typeof(LibraryMarker).Assembly;
    private static readonly string _version = _assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
        .Single(attribute => attribute.Key == "PackageVersion").Value
        ?? throw new InvalidOperationException("Missing fixture package version.");
    private static readonly string _name = $"Consumer.CodeAnalysis.Library-{_version}";
    private static readonly string _packagePath = Path.Combine(AssemblyMetadataUtils.ArtifactsPath,
        "package", "release", $"Consumer.CodeAnalysis.Library.{_version}");

    [Test]
    public async Task Normal_library_package_contains_the_exact_versioned_assembly()
    {
        using var package = ZipFile.OpenRead(_packagePath + ".nupkg");
        var dlls = package.Entries.Where(entry => entry.FullName.EndsWith(".dll", StringComparison.Ordinal)).ToArray();
        await Assert.That(dlls.Length).IsEqualTo(1);
        await Assert.That(dlls[0].FullName).IsEqualTo($"lib/netstandard2.0/{_name}.dll");
        using var stream = dlls[0].Open();
        var packagedHash = SHA256.HashData(stream);
        var compiledHash = SHA256.HashData(File.ReadAllBytes(_assembly.Location));
        await Assert.That(Convert.ToHexString(packagedHash)).IsEqualTo(Convert.ToHexString(compiledHash));
        await Assert.That(package.Entries.Any(entry => entry.FullName.StartsWith("analyzers/", StringComparison.Ordinal))).IsFalse();
    }

    [Test]
    public async Task Symbol_package_contains_the_matching_versioned_pdb()
    {
        using var package = ZipFile.OpenRead(_packagePath + ".snupkg");
        var pdbs = package.Entries.Where(entry => entry.FullName.EndsWith(".pdb", StringComparison.Ordinal)).ToArray();
        await Assert.That(pdbs.Length).IsEqualTo(1);
        await Assert.That(pdbs[0].FullName).IsEqualTo($"lib/netstandard2.0/{_name}.pdb");
    }
}
