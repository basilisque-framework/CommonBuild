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

using System.Reflection;

namespace CommonBuild.Integration.TestSupport.Utils;

/// <summary>
/// Utility class for handling NuGet package operations in integration tests.
/// </summary>
public static class AssemblyMetadataUtils
{
    /// <summary>
    /// Returns the path of the Basilisque.CommonBuild package under test
    /// </summary>
    public static string CommonBuildNuGetPackagePath { get; } = getNuGetPackagePathFromMetadata("PkgBasilisque_CommonBuild");

    /// <summary>
    /// Returns the path of the artifacts folder for the Basilisque.CommonBuild package under test
    /// </summary>
    public static string ArtifactsPath { get; } = getValueFromAssemblyMetadata("ArtifactsPath");

    /// <summary>
    /// Returns the build configuration used for the Basilisque.CommonBuild package under test
    /// </summary>
    public static string BuildConfiguration { get; } = getValueFromAssemblyMetadata("BuildConfiguration");

    private static string getNuGetPackagePathFromMetadata(string key)
    {
        var result = getValueFromAssemblyMetadata(key);

        if (!System.IO.Directory.Exists(result))
            throw new Exception($"The path of the package '{key}' does not exist: '{result}'");

        return result;
    }

    private static string getValueFromAssemblyMetadata(string key)
    {
        var result = System.Reflection.Assembly
            .GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == key)?.Value;

        if (result is null)
            throw new Exception($"The key '{key}' was not found in the assembly metadata.");

        return result;
    }
}