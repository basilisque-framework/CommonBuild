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

    private string getBuildProperty(string propertyName)
    {
        var propertyValue = typeof(CodeAnalysisTestInfo).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(attribute => attribute.Key == propertyName)?.Value;

        if (propertyValue is null)
            throw new Exception($"Build property '{propertyName}' not found.");

        return propertyValue;
    }
}
