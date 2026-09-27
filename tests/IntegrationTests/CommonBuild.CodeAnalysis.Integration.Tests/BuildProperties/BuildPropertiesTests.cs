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

using Consumer.CodeAnalysis;
using System.Reflection;

namespace CommonBuild.CodeAnalysis.Integration.Tests.BuildProperties;

internal class BuildPropertiesTests
{
    [Test]
    public async Task Ensure_IncludeBuildOutput_IsFalse()
    {
        var includeBuildOutput = getBuildProperty("IncludeBuildOutput");

        await Assert.That(includeBuildOutput).IsEqualTo("false");
    }

    [Test]
    public async Task Ensure_AssemblyName_Contains_PackageVersionSuffix()
    {
        var packageVersion = getBuildProperty("PackageVersion");
        var assemblyName = typeof(CodeAnalysisTestInfo).Assembly.GetName().Name;

        await Assert.That(assemblyName).IsNotNull().And.Contains($"-{packageVersion}");
    }

    [Test]
    public async Task Ensure_MarkupCompilePass1DependsOn_Contains_VersioningTarget()
    {
        var dependsOn = getBuildProperty("MarkupCompilePass1DependsOn");

        await Assert.That(dependsOn).Contains("BAS_CB_SetVersionProperties");
    }

    [Test]
    public async Task Ensure_GetPackageVersionDependsOn_Contains_VersioningTarget()
    {
        var dependsOn = getBuildProperty("GetPackageVersionDependsOn");

        await Assert.That(dependsOn).Contains("BAS_CB_SetVersionProperties");
    }

    private string getBuildProperty(string propertyName)
    {
        var propertyValue = typeof(CodeAnalysisTestInfo).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(attribute => attribute.Key == propertyName)?.Value;

        if (propertyValue is null)
            throw new Exception($"Build property '{propertyName}' not found.");

        return propertyValue;
    }
}
