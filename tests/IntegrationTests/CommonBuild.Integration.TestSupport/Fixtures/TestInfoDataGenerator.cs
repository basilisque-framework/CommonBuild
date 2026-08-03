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

using Consumer.Shared;

namespace CommonBuild.Integration.TestSupport.Fixtures;

/// <summary>
/// A data generator for the list of available test fixture information.
/// </summary>
public class TestInfoDataGenerator : DataSourceGeneratorAttribute<ITestInfo>
{
    /// <summary>
    /// Generates data sources for the available test fixture information.
    /// </summary>
    /// <param name="dataGeneratorMetadata">The <see cref="DataGeneratorMetadata"/> instance provided by TUnit, containing metadata for the data generator.</param>
    /// <returns>A list of <see cref="ITestInfo"/> instances representing the available test fixture information.</returns>
    protected override IEnumerable<Func<ITestInfo>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return FixtureInfo.TestFixtureInfos.Select(info => new Func<ITestInfo>(() => info));
    }
}
