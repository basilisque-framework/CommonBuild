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

namespace CommonBuild.Integration.TestSupport.TestSetup;

/// <summary>
/// Prepares unpacked NuGet package contents for integration tests.
/// </summary>
public class UnpackNuGetPackages
{
    private static readonly object _syncRoot = new();
    private static bool _isPrepared;

    private static readonly string _packageArtifactsPath = System.IO.Path.Combine(
        Utils.AssemblyMetadataUtils.ArtifactsPath,
        "package",
        "release"
    );

    private static readonly string _unpackedPackagesPath = System.IO.Path.Combine(
        Utils.AssemblyMetadataUtils.ArtifactsPath,
        "unpacked-packages",
        $"pid-{Environment.ProcessId}"
    );

    /// <summary>
    /// Gets the path where package contents are unpacked for test inspection.
    /// </summary>
    public static string UnpackedPackagesPath => _unpackedPackagesPath;

    /// <summary>
    /// Ensures that all package artifacts are unpacked exactly once for the current test process.
    /// </summary>
    public static void EnsureUnpackedPackagesAvailable()
    {
        lock (_syncRoot)
        {
            if (_isPrepared)
                return;

            cleanup();
            unpack();

            _isPrepared = true;
        }
    }

    /// <summary>
    /// Cleans unpacked package artifacts and resets the setup state.
    /// </summary>
    public static void CleanupUnpackedPackages()
    {
        lock (_syncRoot)
        {
            cleanup();
            _isPrepared = false;
        }
    }

    /// <summary>
    /// TUnit lifecycle hook for preparing package contents before a test session.
    /// </summary>
    [Before(TestSession)]
    public static void BeforeTestSession()
    {
        EnsureUnpackedPackagesAvailable();
    }

    /// <summary>
    /// TUnit lifecycle hook for cleaning unpacked package contents after a test session.
    /// </summary>
    [After(TestSession)]
    public static void AfterTestSession()
    {
        CleanupUnpackedPackages();
    }

    private static void unpack()
    {
        if (!System.IO.Directory.Exists(_packageArtifactsPath))
            throw new System.IO.DirectoryNotFoundException($"Package artifacts path not found: '{_packageArtifactsPath}'.");

        System.IO.Directory.CreateDirectory(_unpackedPackagesPath);

        foreach (var packageFilePath in System.IO.Directory.EnumerateFiles(_packageArtifactsPath, "*.nupkg", System.IO.SearchOption.TopDirectoryOnly))
        {
            var packageDirectoryName = System.IO.Path.GetFileNameWithoutExtension(packageFilePath);
            var packageExtractPath = System.IO.Path.Combine(_unpackedPackagesPath, packageDirectoryName);

            System.IO.Directory.CreateDirectory(packageExtractPath);

            System.IO.Compression.ZipFile.ExtractToDirectory(
                sourceArchiveFileName: packageFilePath,
                destinationDirectoryName: packageExtractPath,
                overwriteFiles: true
            );
        }

    }

    private static void cleanup()
    {
        if (System.IO.Directory.Exists(_unpackedPackagesPath))
            System.IO.Directory.Delete(_unpackedPackagesPath, recursive: true);
    }
}
