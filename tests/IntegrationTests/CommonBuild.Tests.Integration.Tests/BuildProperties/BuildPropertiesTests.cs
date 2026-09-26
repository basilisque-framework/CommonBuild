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

using Consumer.Tests;
using System.Reflection;

namespace CommonBuild.Tests.Integration.Tests.BuildProperties;

internal class BuildPropertiesTests
{
    [Test]
    public async Task TestsProject_HasExpectedProjectTypeProperties()
    {
        var assembly = typeof(TestsTestInfo).Assembly;

        await Assert.That(getBuildProperty(assembly, "IsPackable")).IsEqualTo("false");
        await Assert.That(getBuildProperty(assembly, "IsPublishable")).IsEqualTo("false");
        await Assert.That(getBuildProperty(assembly, "IsTestProject")).IsEqualTo("true");
        await Assert.That(getBuildProperty(assembly, "SonarQubeTestProject")).IsEqualTo("true");
    }

    private static string getBuildProperty(Assembly assembly, string propertyName)
    {
        var propertyValue = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attribute => attribute.Key == propertyName)?.Value;

        if (propertyValue is null)
            throw new Exception($"Build property '{propertyName}' not found for assembly '{assembly.GetName().Name}'.");

        return propertyValue;
    }
}