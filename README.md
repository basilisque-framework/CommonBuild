<!--
   Copyright 2023-2025 Alexander Stärk

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
# Basilisque - Common Build

## Overview
[![NuGet](https://img.shields.io/badge/NuGet-latest-%23004880.svg?logo=nuget)](https://www.nuget.org/packages/Basilisque.CommonBuild)
[![License](https://img.shields.io/badge/License-Apache%20License%202.0-%23D22128.svg?logo=apache&logoColor=%23D22128)](LICENSE.txt)
[![SonarCloud](https://img.shields.io/badge/SonarCloud-main-%23F3702A.svg?logo=sonarcloud&logoColor=%23F3702A)](https://sonarcloud.io/project/overview?id=basilisque-framework_CommonBuild)  

This project provides __optional__ common build configuration for projects that use the Basilisque framework. But it also can be used without Basilique.  
So if you like the contained configuration feel free to use it, otherwise simply don't.

## Description
This project doesn't contain any assemblies that will be deployed with the target project. Instead it only contains build time configuration (.props/.targets-files, ...) to provide a common basic set of configuration to all target projects.

### Usage
Install the [NuGet package](https://www.nuget.org/packages/Basilisque.CommonBuild)  
This will automatically include the contained .props and .targets files in the build.

### Conventions
The configuration is based on conventions regarding the names of the target projects. _(The folder structure of the solution is irrelevant.)_

>__ExampleSolution__
>- MyProject.[__Service__](#servicesConfig)  
>_<sup>Containing the startup code of a backend (Windows) service<sup>_
>- MyProject.Service.[__Tests__](#testsConfig)  
>_<sup>Containing automated tests. In this case for the MyProject.Service-project<sup>_
>- MyProject.[__API__](#apiConfig)  
>_<sup>Containing the public facing code of the backend (API-Controllers, ...)<sup>_
>- MyProject.API.[__Tests__](#testsConfig)  
>_<sup>Containing automated tests. In this case for the MyProject.API-project<sup>_
>- MyProject.[__Domain__](#domainConfig)  
>_<sup>Containing the business logic<sup>_
>- MyProject.Domain.[__Tests__](#testsConfig)  
>_<sup>Containing automated tests. In this case for the MyProject.Domain-project<sup>_
>- MyProject.[__DataAccess__](#dataAccessConfig)  
>_<sup>Containing the data access layer (e.g. using Entity Framework)<sup>_
>- MyProject.DataAccess.[__Tests__](#testsConfig)  
>_<sup>Containing automated tests. In this case for the MyProject.DataAccess-project<sup>_
>- MyProject.[__Benchmarks__](#benchmarksConfig)  
>_<sup>Containing benchmarks for a project<sup>_
>- MyProject.[__CodeAnalysis__](#codeAnalysisConfig)  
>_<sup>Containing code analysis support for the project like source generators, analyzers and fixes<sup>_

Obviously not all applications need all of those project types. So e.g. if you don't need data access, then just do not create a project named like it. But if you do need data access, then let the project name end with _.DataAccess_. That is the intention behind the configuration in Basilisque.CommonBuild.

### Provided Configuration
__.props / .targets__
- <a name="generalConfig"></a>__General__ (for all project types)
  | Property 	                | Value                       | Remark                                                    |
  |-------------------------- |---------------------------- |---------------------------------------------------------- |
  | Nullable                  | enable                      |                                                           |
  | ImplicitUsings            | disable                     |                                                           |
  | NeutralLanguage           | en-US                       |                                                           |
  | Title                     | \<AssemblyName>             | will only be set when the title is empty                  |
  | Copyright                 | Copyright © yyyy \<Company> | will only be set when the copyright is empty.<br/>When the company is empty, authors will be used instead.<br/>In addition you can set the property BAS_CB_Copyright_BeginYear to the begin year of the copyright to show a year range. |
  | Company                   | \<Authors>                  | will only be set when the authors property is not empty   |
  | Product	                  | \<AssemblyName>             |                                                           |
  | IsPackable                | false                       | will be set when Configuration != Release                 |
  | GenerateDocumentationFile | true                        | not for .Tests-projects and .Benchmarks-projects          |
  | LangVersion               | 11.0                        | will only be set when TargetFramework is netstandard2.0<br/>(mainly for nullable reference types in C# 8 and raw string literals in C# 11)   |
  | Global Usings             | System<br/>System.Collections.Generic<br/>System.Linq<br/>System.Threading.Tasks  | The flag for this setting is named BAS_CB_Add_GlobalUsings   |
  | AssemblyVersion           | Major.Minor                 | see [versioning](#versioning)                             |
  | FileVersion               | Major.Minor(.Build)         | see [versioning](#versioning)                             |
  | InformationalVersion      | Major.Minor(.Build)(-Suffix(Revision)) | see [versioning](#versioning)                  |
  | PackageVersion            | Major.Minor(.Build)(-Suffix(Revision)) | see [versioning](#versioning)                  |
<!--
- <a name="servicesConfig"></a>__*.Service__
   - ???
- <a name="apiConfig"></a>__*.API__
   - ???
- <a name="domainConfig"></a>__*.Domain__
   - ???
- <a name="dataAccessConfig"></a>__*.DataAccess__
   - ???
-->
- <a name="testsConfig"></a>__*.Tests__
  | Property            | Value                                           | Remark                                              |
  |-------------------- |------------------------------------------------ |---------------------------------------------------- |
  | IsPackable          | false                                           |                                                     |
  | IsPublishable       | false                                           |                                                     |
  | IsTestProject       | true                                            |                                                     |
  | SonarQubeTestProject| true                                            |                                                     |
  | RunSettingsFilePath | [runsettings in this package](#testRunsettings) | will only be set when RunSettingsFilePath is empty  |
  | Global Usings       | Microsoft.VisualStudio.TestTools.UnitTesting    | will only be set when MSTest is referenced          |
- <a name="benchmarksConfig"></a>__*.Benchmarks__
  | Property              | Value  | Remark |
  |---------------------- |------- |------- |
  | IsPackable            | false  |        |
  | IsPublishable         | false  |        |
  | SonarQubeTestProject  | true   |        |
- <a name="codeAnalysisConfig"></a>__*.CodeAnalysis__
  | Property 	          | Value                     | Remark                                                    |
  |-------------------- |-------------------------- | --------------------------------------------------------- |
  | Global Usings       | Microsoft.CodeAnalysis    |                                                           |
  | IncludeBuildOutput  | false                     | do not pack analyzers/source generators as lib dependency |
  | AssemblyName        | \<AssemblyName>-Major.Minor(.Build)(-Suffix(Revision)) | Version is appended to assembly name to fix loading of dependencies in Visual Studio. Can be disabled by settings BAS_CB_Set_AssemblyName_VersionSuffix to false |

  Analyzers and Source Generators will be packed to the analyzer directory of the NuGet package instead of being packed as normal lib dependency. Can be disabled by setting BAS_CB_CodeAnalysis_PackCS to false.

If you don't want a specific property to be set, you can prevent it. For every property there is a corresponding flag that controls, if the property will be set or not.  
The flags are named BAS_CB_Set_\<PropertyName\>.  
For example if you don't want the NeutralLanguage to be set, add this to your project:

    <PropertyGroup>
      <BAS_CB_Set_NeutralLanguage>false</BAS_CB_Set_NeutralLanguage>
    </PropertyGroup>

<a name="testRunsettings"></a>__Test runsettings__  
Provides a default .runsettings-file for tests and coverage. This is automatically set to RunSettingsFilePath when RunSettingsFilePath is empty. So when you want to use your own runsettings, simply overwrite RunSettingsFilePath like you normally would do anyway.

Included settings:
- MaxCpuCount = 0  
max process-level parallelization; but parallelization on thread level is not enforced within the test dll.
- TreatNoTestsAsError = true  
when no tests are found in the test project, it is treated as error. So e.g. when, due to an error, no tests are found in your test project, your test run will fail.
- excludes \*.Tests.dll, \*.Benchmarks.dll and Microsoft.\*.dll projects from code coverage

<a name="versioning"></a>__Versioning__  
The versions are built from the properties BAS_CB_BuildType, BAS_CB_VersionMajor, BAS_CB_VersionMinor, BAS_CB_VersionBuild and BAS_CB_VersionRevision. If you want to disable the versioning you can set BAS_CB_Set_Versions = false.

The build type differentiates between release builds and different kinds of prerelease builds.
Recommended values are:
| Value   | Usage                                           |
|-------- |------------------------------------------------ |
| Alpha   | Local builds                                    |
| CI      | Continuous integration builds or nightly builds |
| Preview | Public preview builds                           |
| RC      | Release candidates                              |
| Release | Release builds                                  |

But you can use any other string. The only fixed value is 'Release'. This removes the prerelease suffix and the revision from the InformationalVersion and the PackageVersion. Any other string will be used as suffix including the revision number.  
Be aware, that the suffixes will be ordered alphabetically e.g. by NuGet. So for example version 1.0-RC is considered higher than a version 1.0-Preview, because it is further back in the alphabet.

This is how the version properties are being set:
| Property 	            | Value                                   |
|---------------------- |---------------------------------------- |
| AssemblyVersion       | Major.Minor                             |
| FileVersion           | Major.Minor(.Build)                     |
| InformationalVersion  | Major.Minor(.Build)(-Suffix(Revision))  |
| PackageVersion        | Major.Minor(.Build)(-Suffix(Revision))  |

You can specify the necessary properties directly in your project or you can set them using dotnet build (MSBuild) parameters:

    dotnet build MySolution.sln -p:BAS_CB_VersionMajor=2 -p:BAS_CB_VersionMinor=1 -p:BAS_CB_VersionBuild=0 -p:BAS_CB_VersionRevision=1043 -p:BAS_CB_BuildType=CI

Typically you would set them on the build server using parameters. Local builds in the IDE won't have any parameters set.
When nothing else is specified, build type 'Alpha' is used as fallback. By doing so, all local builds will be in this category.
The purpose for this is, that almost all other build types (suffixes) would be considered higher than a local build, because A is right at the beginning of the alphabet.

Major and Minor are set to 1.0 when empty, Build and Revision are omitted from the version when empty.

### Logging
If you want to log the properties, that are set by this project, you can set the property BAS_CB_Log_Properties to true. This will log the current value of all these properties after the _PrepareForBuild_ target.

    <PropertyGroup>
      <BAS_CB_Log_Properties>true</BAS_CB_Log_Properties>
    </PropertyGroup>

## License
The Basilisque framework (including this repository) is licensed under the [Apache License, Version 2.0](LICENSE.txt).