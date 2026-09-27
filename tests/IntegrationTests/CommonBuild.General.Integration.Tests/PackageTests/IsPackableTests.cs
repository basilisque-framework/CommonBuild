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
using Consumer.Shared;
using System.Reflection;

namespace CommonBuild.General.Integration.Tests.PackageTests;

internal class IsPackableTests
{
    private static string _artifactsPath = System.IO.Path.Combine(
        CommonBuild.Integration.TestSupport.Utils.AssemblyMetadataUtils.ArtifactsPath,
        "package",
        "release"//CommonBuild.Integration.TestSupport.Utils.AssemblyMetadataUtils.BuildConfiguration.ToLower() //default 'dotnet pack' is always 'release'
        );

    [Test]
    public async Task ArtifactsPackageDirectory_Contains_TheRightAmountOfPackageFiles()
    {
        var expectedFileCount = FixtureInfo.TestFixtureInfos.Where(fi => fi.IsPackable).Count();

        var fixturePackageNames = FixtureInfo.TestFixtureInfos
            .Select(getPackageFileName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var actualFileCount = System.IO.Directory
            .EnumerateFiles(_artifactsPath, "*.nupkg", System.IO.SearchOption.TopDirectoryOnly)
            .Count(path => fixturePackageNames.Contains(System.IO.Path.GetFileName(path)));

        await Assert.That(actualFileCount).IsEqualTo(expectedFileCount);
    }

    [Test]
    [TestInfoDataGenerator]
    public async Task ArtifactsPackageDirectory_Contains_TheCorrectPackages(ITestInfo info)
    {
        var packageFileName = getPackageFileName(info);

        var matchingPackagesCount = System.IO.Directory.EnumerateFiles(_artifactsPath, packageFileName).Count();

        var expectedPackageCount = info.IsPackable ? 1 : 0;

        await Assert.That(matchingPackagesCount).IsEqualTo(expectedPackageCount);
    }

    private static string getPackageFileName(ITestInfo info)
    {
        var packageVersion = info.Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(attribute => attribute.Key == "PackageVersion").Value;

        if (string.IsNullOrWhiteSpace(packageVersion))
            throw new InvalidOperationException("The fixture package version must not be empty.");

        return $"{FixtureInfo.GetProjectName(info.Assembly)}.{packageVersion}.nupkg";
    }
}
