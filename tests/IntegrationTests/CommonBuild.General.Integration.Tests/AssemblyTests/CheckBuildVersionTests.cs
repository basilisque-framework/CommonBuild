/*
   Copyright 2026 Alexander Stärk

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Reflection;

namespace CommonBuild.General.Integration.Tests.AssemblyTests;

/// <summary>
/// Since the tests in <see cref="AssemblyVersioningTests"/> use the assembly version of this build as reference value, it needs to be ensured, that the version is somehow filled in a meaningful way.
/// </summary>
internal class CheckBuildVersionTests
{
    // get the version of this integration test assembly because this is built with the same version information than the rest of the repository
    private System.Version _thisAssemblyVersion = typeof(CheckBuildVersionTests).Assembly.GetName().Version!;
    private string? _thisAssemblyFileVersion = typeof(CheckBuildVersionTests).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
    private string? _thisAssemblyInformationalVersion = typeof(CheckBuildVersionTests).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    private string? _thisAssemblyPackageVersion = typeof(CheckBuildVersionTests).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(attribute => attribute.Key == "PackageVersion")?.Value;
    private string? _defineConstants = typeof(CheckBuildVersionTests).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(attribute => attribute.Key == "DefineConstants")?.Value;

    [Test]
    public async Task Ensure_CurrentBuild_AssemblyVersion_Contains_VersionInformation()
    {
        // ensure the assembly version is filled at all
        var emptyVersion = new Version();
        await Assert.That(_thisAssemblyVersion).IsNotNull().And.IsGreaterThan(emptyVersion);

        // ensure the version is greater than 1
        // (At the time of writing this code the repo is already at version 2 and therefore never will be at version 1 or lower.
        // So this test can ensure that the build has set at lease some version information and not the default value.)
        await Assert.That(_thisAssemblyVersion!.Major).IsGreaterThan(1);
    }

    [Test]
    public async Task Ensure_CurrentBuild_AssemblyVersion_RevisionIsEmpty()
    {
        // since the build system is using semver the revision is not used and should be empty (semver has only 3 digits, major.minor.patch)
        await Assert.That(_thisAssemblyVersion.Revision).IsEqualTo(0);
    }

    [Test]
    public async Task Ensure_CurrentBuild_AssemblyVersion_HasNotMoreThan2Digits()
    {
        // semver has 3 digits, major.minor.patch
        // Patch means backwards compatible bug fixes, so it is not used in the assembly version to make them load without problems in patches.

        await Assert.That(_thisAssemblyVersion.Major).IsGreaterThan(0);
        await Assert.That(_thisAssemblyVersion.Minor).IsGreaterThanOrEqualTo(0);
        await Assert.That(_thisAssemblyVersion.Build).IsEqualTo(0);
        await Assert.That(_thisAssemblyVersion.Revision).IsEqualTo(0);
    }

    [Test]
    public async Task Ensure_CurrentBuild_FileVersion_HasExactly3Digits()
    {
        await Assert.That(_thisAssemblyFileVersion).IsNotNull();

        var fileVersionParts = _thisAssemblyFileVersion!.Split('.');

        await Assert.That(fileVersionParts.Length).IsEqualTo(3);

        var fileVersionDigits = fileVersionParts.Select(part => int.TryParse(part, out var digit) ? digit : -1).ToArray();

        await Assert.That(fileVersionDigits[0]).IsGreaterThan(0);
        await Assert.That(fileVersionDigits[1]).IsGreaterThanOrEqualTo(0);
        await Assert.That(fileVersionDigits[2]).IsGreaterThanOrEqualTo(0);
    }

    [Test]
    public async Task Ensure_CurrentBuild_PackageVersion_HasExactly3Digits()
    {
        await Assert.That(_thisAssemblyPackageVersion).IsNotNull();

        var packageVersionParts = _thisAssemblyPackageVersion!.Split('.');

        await Assert.That(packageVersionParts.Length).IsEqualTo(3);

        var packageVersionDigits = packageVersionParts.Select(part => int.TryParse(part, out var digit) ? digit : -1).ToArray();

        await Assert.That(packageVersionDigits[0]).IsGreaterThan(0);
        await Assert.That(packageVersionDigits[1]).IsGreaterThanOrEqualTo(0);
        await Assert.That(packageVersionDigits[2]).IsGreaterThanOrEqualTo(0);
    }

    [Test]
    public async Task Ensure_CurrentBuild_InformationalVersion_HasExactly3Digits()
    {
        var (versionInformation, _) = await getInformationalVersionParts();

        var version = removePrereleaseInformation(versionInformation);

        var versionParts = version.Split('.');

        await Assert.That(versionParts.Length).IsEqualTo(3);

        var versionDigits = versionParts.Select(part => int.TryParse(part, out var digit) ? digit : -1).ToArray();

        await Assert.That(versionDigits[0]).IsGreaterThan(0);
        await Assert.That(versionDigits[1]).IsGreaterThanOrEqualTo(0);
        await Assert.That(versionDigits[2]).IsGreaterThanOrEqualTo(0);
    }

    [Test]
    public async Task Ensure_CurrentBuild_InformationalVersion_HasGitHash()
    {
        var (_, fullGitHash) = await getInformationalVersionParts();

        await Assert.That(fullGitHash).IsNotNull().And.IsNotEmpty();
    }

    [Test]
    public async Task Ensure_CurrentBuild_PackageAndInformationalVersion_FollowBuildTypeRules()
    {
        var (informationalVersionBase, _) = await getInformationalVersionParts();
        var buildType = getBuildTypeFromDefineConstants();

        if (buildType == "Release")
        {
            await Assert.That(informationalVersionBase).DoesNotContain("-");
            return;
        }

        var suffix = $"-{buildType}";
        var separatorIndex = informationalVersionBase.IndexOf(suffix, StringComparison.Ordinal);

        await Assert.That(separatorIndex).IsGreaterThan(0);

        var revisionPart = informationalVersionBase[(separatorIndex + suffix.Length)..];

        await Assert.That(revisionPart.Length).IsEqualTo(5);
        await Assert.That(revisionPart.All(char.IsDigit)).IsTrue();
    }

    private async Task<(string versionInformation, string fullGitHash)> getInformationalVersionParts()
    {
        await Assert.That(_thisAssemblyInformationalVersion).IsNotNull();

        var result = _thisAssemblyInformationalVersion!.Split('+');

        await Assert.That(result.Length).IsEqualTo(2);

        return (result[0], result[1]);
    }

    private string removePrereleaseInformation(string versionInformation)
    {
        var index = versionInformation.IndexOf('-');
        if (index >= 0)
            return versionInformation.Substring(0, index);

        return versionInformation;
    }

    private string getBuildTypeFromDefineConstants()
    {
        if (string.IsNullOrWhiteSpace(_defineConstants))
            throw new Exception("Build property 'DefineConstants' not found.");

        if (_defineConstants.Contains("BUILD_TYPE_RELEASE", StringComparison.Ordinal)) return "Release";
        if (_defineConstants.Contains("BUILD_TYPE_RC", StringComparison.Ordinal)) return "RC";
        if (_defineConstants.Contains("BUILD_TYPE_PREVIEW", StringComparison.Ordinal)) return "Preview";
        if (_defineConstants.Contains("BUILD_TYPE_CI", StringComparison.Ordinal)) return "CI";
        if (_defineConstants.Contains("BUILD_TYPE_ALPHA", StringComparison.Ordinal)) return "Alpha";

        throw new Exception("Could not determine build type from DefineConstants.");
    }
}
