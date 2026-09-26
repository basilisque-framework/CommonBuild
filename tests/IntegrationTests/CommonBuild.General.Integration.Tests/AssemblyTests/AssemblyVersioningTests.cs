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

using CommonBuild.Integration.TestSupport.Fixtures;
using System.Diagnostics;
using System.Reflection;

namespace CommonBuild.General.Integration.Tests.AssemblyTests;

internal class AssemblyVersioningTests
{
    private readonly Assembly _referenceAssembly = typeof(AssemblyVersioningTests).Assembly;

    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task AssemblyVersion_HasMajorAndMinorOnly(Assembly assembly)
    {
        var version = assembly.GetName().Version;

        await Assert.That(version).IsNotNull();

        await Assert.That(version!.Major).IsGreaterThan(0);
        await Assert.That(version.Minor).IsGreaterThanOrEqualTo(0);
        await Assert.That(version.Build).IsEqualTo(0);
        await Assert.That(version.Revision).IsEqualTo(0);
    }

    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task FileVersion_HasExactlyThreeNumericSegments(Assembly assembly)
    {
        var fileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        await Assert.That(fileVersion).IsNotNull();

        var parts = fileVersion!.Split('.');

        await Assert.That(parts.Length).IsEqualTo(3);

        var digits = parts.Select(part => int.TryParse(part, out var value) ? value : -1).ToArray();

        await Assert.That(digits[0]).IsGreaterThan(0);
        await Assert.That(digits[1]).IsGreaterThanOrEqualTo(0);
        await Assert.That(digits[2]).IsGreaterThanOrEqualTo(0);
    }

    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task InformationalVersion_HasSemVerAndGitHash(Assembly assembly)
    {
        var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        await Assert.That(informationalVersion).IsNotNull();

        var informationalVersionParts = informationalVersion!.Split('+');
        await Assert.That(informationalVersionParts.Length).IsEqualTo(2);

        var semVerWithOptionalSuffix = informationalVersionParts[0];
        var gitHash = informationalVersionParts[1];

        await Assert.That(gitHash).IsNotEmpty();

        var semVer = stripPreReleaseSuffix(semVerWithOptionalSuffix);
        var semVerSegments = semVer.Split('.');

        await Assert.That(semVerSegments.Length).IsEqualTo(3);

        var semVerDigits = semVerSegments.Select(part => int.TryParse(part, out var value) ? value : -1).ToArray();

        await Assert.That(semVerDigits[0]).IsGreaterThan(0);
        await Assert.That(semVerDigits[1]).IsGreaterThanOrEqualTo(0);
        await Assert.That(semVerDigits[2]).IsGreaterThanOrEqualTo(0);
    }

    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task InformationalVersion_IsIdentical_ToProductVersion(Assembly assembly)
    {
        var productVersion = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

        var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        await Assert.That(productVersion).IsEqualTo(informationalVersion);
    }

    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task VersionBase_IsConsistent_WithCurrentBuild(Assembly assembly)
    {
        var referenceAssemblyVersion = _referenceAssembly.GetName().Version;
        var fixtureAssemblyVersion = assembly.GetName().Version;

        await Assert.That(referenceAssemblyVersion).IsNotNull();
        await Assert.That(fixtureAssemblyVersion).IsNotNull();

        await Assert.That(fixtureAssemblyVersion!.Major).IsEqualTo(referenceAssemblyVersion!.Major);
        await Assert.That(fixtureAssemblyVersion.Minor).IsEqualTo(referenceAssemblyVersion.Minor);

        var referenceFileVersion = _referenceAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        var fixtureFileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        await Assert.That(referenceFileVersion).IsNotNull();
        await Assert.That(fixtureFileVersion).IsNotNull();

        var referenceFileVersionBase = getVersionBase(referenceFileVersion!);
        var fixtureFileVersionBase = getVersionBase(fixtureFileVersion!);

        await Assert.That(fixtureFileVersionBase).IsEqualTo(referenceFileVersionBase);

        var referenceInformationalVersion = _referenceAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var fixtureInformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        await Assert.That(referenceInformationalVersion).IsNotNull();
        await Assert.That(fixtureInformationalVersion).IsNotNull();

        var referenceInformationalVersionBase = getInformationalVersionBase(referenceInformationalVersion!);
        var fixtureInformationalVersionBase = getInformationalVersionBase(fixtureInformationalVersion!);

        await Assert.That(fixtureInformationalVersionBase).IsEqualTo(referenceInformationalVersionBase);
    }

    private static string stripPreReleaseSuffix(string version)
    {
        var prereleaseSeparatorIndex = version.IndexOf('-');
        if (prereleaseSeparatorIndex < 0)
            return version;

        return version.Substring(0, prereleaseSeparatorIndex);
    }

    private static string getVersionBase(string version)
    {
        var parts = version.Split('.');
        if (parts.Length < 3)
            throw new Exception($"Version '{version}' does not contain a major.minor.patch base.");

        return string.Join('.', parts.Take(3));
    }

    private static string getInformationalVersionBase(string informationalVersion)
    {
        var semVerWithOptionalSuffix = informationalVersion.Split('+')[0];
        var semVer = stripPreReleaseSuffix(semVerWithOptionalSuffix);

        return getVersionBase(semVer);
    }
}
