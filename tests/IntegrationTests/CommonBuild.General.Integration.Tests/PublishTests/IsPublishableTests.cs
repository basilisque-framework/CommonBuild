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

namespace CommonBuild.General.Integration.Tests.PackageTests;

internal class IsPublishableTests
{
    private static string _artifactsPath = System.IO.Path.Combine(CommonBuild.Integration.TestSupport.Utils.AssemblyMetadataUtils.ArtifactsPath, "publish");

    [Test]
    public async Task ArtifactsPublishDirectory_Contains_TheRightAmountOfPackageSubdirectories()
    {
        var expectedSubdirectoryCount = FixtureInfo.TestFixtureInfos.Where(fi => fi.IsPublishable).Count();

        var actualSubdirectoryCount = System.IO.Directory.EnumerateDirectories(_artifactsPath).Count();

        await Assert.That(actualSubdirectoryCount).IsEqualTo(expectedSubdirectoryCount);
    }

    [Test]
    [TestInfoDataGenerator]
    public async Task ArtifactsPublishDirectory_Contains_TheCorrectSubdirectories(ITestInfo info)
    {
        var expectedName = FixtureInfo.GetProjectName(info.Assembly);

        await Assert.That(expectedName).IsNotNull();

        var matchingSubdirectoriesCount = System.IO.Directory.EnumerateDirectories(_artifactsPath, expectedName!).Count();

        var expectedSubdirectoriesCount = info.IsPublishable ? 1 : 0;

        await Assert.That(matchingSubdirectoriesCount).IsEqualTo(expectedSubdirectoriesCount);
    }
}
