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
using CommonBuild.Integration.TestSupport.Fixtures;
using System.Reflection;
/*
Do NOT add the followign namespaces to not break the test for implicit global usings:
- System
- System.Collections.Generic
- System.Linq
- System.Threading.Tasks
*/

namespace CommonBuild.General.Integration.Tests.BuildProperties;

internal class BuildPropertiesTests
{
    [Test]
    public async Task Ensure_Custom_GlobalUsings_AreSet()
    {
        // When this test even compiles, it means that the custom global usings are
        // set correctly since those usings are not set in this source file.

        await Assert.That(DateTime.Now).IsGreaterThan(DateTime.MinValue);
        await Assert.That(new List<int>()).IsNotNull();
        await Assert.That(new List<int>().Where(i => true).Any()).IsFalse();
        await Assert.That(Task.CompletedTask).ThrowsNothing();
    }

    [Test]
    public async Task Ensure_Nullable_IsEnabled()
    {
        var nullableEnabled = getBuildProperty("Nullable");

        await Assert.That(nullableEnabled).IsEqualTo("enable");
    }

    [Test]
    public async Task Ensure_ImplicitUsings_IsDisabled()
    {
        var implicitUsingsEnabled = getBuildProperty("ImplicitUsings");

        await Assert.That(implicitUsingsEnabled).IsEqualTo("disable");
    }

    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task Ensure_NeutralLanguage_IsSetTo_enUS(Assembly assembly)
    {
        await Assert.That(getBuildProperty(assembly, "NeutralLanguage")).IsEqualTo("en-US");
    }

    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task Ensure_Company_IsSetTo_Authors(Assembly assembly)
    {
        await Assert.That(getBuildProperty(assembly, "Company")).IsEqualTo("Alexander Stärk");
    }

    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task Ensure_Product_IsSetTo_UnversionedAssemblyName(Assembly assembly)
    {
        await Assert.That(getBuildProperty(assembly, "Product")).IsEqualTo(FixtureInfo.GetProjectName(assembly));
    }

    private string getBuildProperty(string propertyName)
    {
        return getBuildProperty(typeof(CheckBuildVersionTests).Assembly, propertyName);
    }

    private static string getBuildProperty(Assembly assembly, string propertyName)
    {
        var propertyValue = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(attribute => attribute.Key == propertyName)?.Value;

        if (propertyValue is null)
            throw new Exception($"Build property '{propertyName}' not found.");

        return propertyValue;
    }
}
