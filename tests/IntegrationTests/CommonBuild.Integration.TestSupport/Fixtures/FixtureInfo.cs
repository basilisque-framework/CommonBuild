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

using Consumer.Benchmarks;
using Consumer.CodeAnalysis;
using Consumer.Shared;
using Consumer.Tests;

namespace CommonBuild.Integration.TestSupport.Fixtures;

/// <summary>
/// Provides information about the test fixture assemblies
/// </summary>
public static class FixtureInfo
{
    private static List<ITestInfo> _testFixtureInfos = new()
    {
        new BenchmarksTestInfo(),
        new CodeAnalysisTestInfo(),
        new TestsTestInfo()
    };

    private static List<System.Reflection.Assembly> _testFixtureAssemblies = _testFixtureInfos.Select(ti => ti.Assembly).ToList();

    /// <summary>
    /// A list of test info instances for all test fixture assemblies
    /// </summary>
    public static IEnumerable<ITestInfo> TestFixtureInfos => _testFixtureInfos;

    /// <summary>
    /// A list of all test fixtures assemblies
    /// </summary>
    public static IEnumerable<System.Reflection.Assembly> TestFixtureAssemblies => _testFixtureAssemblies;

    /// <summary>
    /// Gets the fixture project name, independently of assembly version suffixes.
    /// </summary>
    public static string GetProjectName(System.Reflection.Assembly assembly)
    {
        return _testFixtureInfos.Single(info => info.Assembly == assembly).GetType().Namespace
            ?? throw new InvalidOperationException("The fixture type must have a project namespace.");
    }
}
