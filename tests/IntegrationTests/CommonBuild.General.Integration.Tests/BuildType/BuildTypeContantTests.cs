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

using CommonBuild.General.Integration.Tests.AssemblyTests;
using System.Reflection;

namespace CommonBuild.General.Integration.Tests.BuildType;

internal class BuildTypeContantTests
{
    private string? _defineConstants = typeof(CheckBuildVersionTests).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(attribute => attribute.Key == "DefineConstants")?.Value;

    [Test]
    public async Task Ensure_DefineConstants_MSBuildVariable_Contains_BuildType()
    {
        await Assert.That(_defineConstants).IsNotNull();

        await Assert.That(_defineConstants).Contains(";BUILD_TYPE_");
    }

    [Test]
    public async Task Ensure_BuildType_Constant_IsSet_DuringBuild()
    {
        // The build type is set via MSBuild define constants, which are set during the build process.
        // It should also be available for the build of this test, so ensure it is.

        string? buildType = null;

#if BUILD_TYPE_ALPHA
        buildType = "Alpha";
#elif BUILD_TYPE_CI
        buildType = "CI";
#elif BUILD_TYPE_PREVIEW
        buildType = "Preview";
#elif BUILD_TYPE_RC
        buildType = "RC";
#elif BUILD_TYPE_RELEASE
        buildType = "Release";
#endif

        await Assert.That(buildType).IsNotNull();
    }
}
