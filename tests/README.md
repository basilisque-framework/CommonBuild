<!--
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
-->
# Basilisque - Common Build Tests

## Overview
This document describes the local workflow for running the integration tests in this folder.

The process should always follow this order:
1. Build and pack the main solution under src
2. Build, publish, and pack the test solution
3. Run the tests

## Workflow
### 1) Build producer solution under src
First, build the CommonBuild producer solution.

    dotnet build src/Basilisque.CommonBuild.slnx -c Release

### 2) Pack producer solution under src
Then create the NuGet packages of the producer solution.

    dotnet pack src/Basilisque.CommonBuild.slnx -c Release --no-build

### 3) Build tests
Build the test solution in the tests folder.

    dotnet build tests/Basilisque.CommonBuild.Tests.slnx -c Release

### 4) Publish tests
Publish the test solution.

    dotnet publish tests/Basilisque.CommonBuild.Tests.slnx -c Release --no-build

### 5) Pack tests
Pack the test-related projects that are packable.

    dotnet pack tests/Basilisque.CommonBuild.Tests.slnx -c Release --no-build

### 6) Run tests
Finally, run the tests with dotnet test.

    dotnet test --solution tests/Basilisque.CommonBuild.Tests.slnx -c Release --no-build

## Complete Example Sequence
The following commands can be executed in order from the repository root:

    dotnet build src/Basilisque.CommonBuild.slnx -c Release
    dotnet pack src/Basilisque.CommonBuild.slnx -c Release --no-build
    dotnet build tests/Basilisque.CommonBuild.Tests.slnx -c Release
    dotnet publish tests/Basilisque.CommonBuild.Tests.slnx -c Release --no-build
    dotnet pack tests/Basilisque.CommonBuild.Tests.slnx -c Release --no-build
    dotnet test --solution tests/Basilisque.CommonBuild.Tests.slnx -c Release --no-build

## Notes
- The commands are intended to be run from the repository root.
- Package-count tests check the exact package version captured from each fixture build. Packages from older builds may remain in the output directory; they do not replace a missing current package.
- For a clean rebuild, you can run a clean step first:

      dotnet clean src/Basilisque.CommonBuild.slnx -c Release
      dotnet clean tests/Basilisque.CommonBuild.Tests.slnx -c Release

