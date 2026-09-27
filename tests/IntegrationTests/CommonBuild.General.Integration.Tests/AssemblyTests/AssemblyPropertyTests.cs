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
using System.Reflection;
using System.Resources;

namespace CommonBuild.General.Integration.Tests.AssemblyTests;

internal class AssemblyPropertyTests
{
    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task NeutralLanguage_IsSetTo_enUS(Assembly assembly)
    {
        string? neutralLanguage = assembly.GetCustomAttribute<NeutralResourcesLanguageAttribute>()?.CultureName;

        await Assert.That(neutralLanguage).IsEqualTo("en-US");
    }

    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task Title_IsSetTo_UnversionedAssemblyName(Assembly assembly)
    {
        string? title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        string assemblyName = FixtureInfo.GetProjectName(assembly);

        await Assert.That(title).IsEqualTo(assemblyName);
    }

    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task Company_IsSetTo_Authors(Assembly assembly)
    {
        string? company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;

        await Assert.That(company).IsEqualTo("Alexander Stärk");
    }

    [Test]
    [TestFixtureAssembliesDataGenerator]
    public async Task Product_IsSetTo_UnversionedAssemblyName(Assembly assembly)
    {
        string? product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
        string assemblyName = FixtureInfo.GetProjectName(assembly);

        await Assert.That(product).IsEqualTo(assemblyName);
    }
}
