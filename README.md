<!--
   Copyright 2023 Alexander Stärk

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
[![NuGet](https://img.shields.io/badge/NuGet-latest-blue.svg)](https://www.nuget.org/packages/Basilisque.CommonBuild)
[![License](https://img.shields.io/badge/License-Apache%20License%202.0-red.svg)](LICENSE.txt)  
This project provides __optional__ common build configuration for projects that use the Basilisque framework. But it also can be used without Basilique.  
So if you like the contained configuration feel free to use it, otherwise simply don't.

## Description
This project doesn't contain any assemblies that will be deployed with the target project. Instead it only contains build time configuration (.props/.targets-files) to provide a common basic set of configuration to all target projects.

### Usage
Install the [NuGet package](https://www.nuget.org/packages/Basilisque.CommonBuild)  
This will automatically include the contained .props and .targets files in the build.

### Conventions
The configuration is based on conventions regarding the names of the target projects. _(The folder structure of the solution is irrelevant.)_

<!--
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
-->
>__ExampleSolution__
>- MyProject.[__Benchmarks__](#benchmarksConfig)  
>_<sup>Containing benchmarks for a project<sup>_
>- MyProject.[__CodeAnalysis__](#codeAnalysisConfig)  
>_<sup>Containing code analysis support for the project like source generators, analyzers and fixes<sup>_
Obviously not all applications need all of those project types. So e.g. if you don't need data access, then just do not create a project named like it. But if you do need data access, then let the project name end with _.DataAccess_. That is the intention behind the configuration in Basilisque.CommonBuild.

### Provided Configuration
__.props / .targets__
- <a name="generalConfig"></a>__General__ (for all project types)
  | Property 	                | Value                       | Remark                                                    |
  |-----------------          |---------------------------- | --------------------------------------------------------- |
  | Nullable                  | enable                      |                                                           |
  | ImplicitUsings            | disable                     |                                                           |
  | NeutralLanguage           | en-US                       |                                                           |
  | Title	                    | \<AssemblyName>             | will only be set when the title is empty                  |
  | Copyright                 | Copyright © yyyy \<Company> | will only be set when the copyright is empty.<br/>When the company is empty, authors will be used instead.<br/>In addition you can set the property BAS_CB_Copyright_BeginYear to the begin year of the copyright to show a year range. |
  | Company                   | \<Authors>                  | will only be set when the authors property is not empty   |
  | Product	                  | \<AssemblyName>             |                                                           |
  | PackageId	                | \<AssemblyName>             |                                                           |
  | IsPackable                | false                       | will be set when Configuration != Release                 |
  | GenerateDocumentationFile | true                        | not for .Tests-projects and .Benchmarks-projects          |
  | LangVersion               | 11.0                        | will only be set when TargetFramework is netstandard2.0<br/>(mainly for nullable reference types in C# 8 and raw string literals in C# 11)   |
  | Global Usings             | System<br/>System.Collections.Generic<br/>System.Linq<br/>System.Threading.Tasks  | The flag for this setting is named BAS_CB_Add_GlobalUsings   |
<!--
- <a name="servicesConfig"></a>__Service__
   - ???
- <a name="apiConfig"></a>__API__
   - ???
- <a name="domainConfig"></a>__Domain__
   - ???
- <a name="dataAccessConfig"></a>__DataAccess__
   - ???
-->
- <a name="testsConfig"></a>__Tests__
  | Property            | Value                       | Remark                                              |
  |-------------------- |---------------------------- |---------------------------------------------------- |
  | IsPackable          | false                       |                                                     |
- <a name="benchmarksConfig"></a>__Benchmarks__
  | Property   | Value  | Remark |
  |----------- |------- |------- |
  | IsPackable | false  |        |
- <a name="codeAnalysisConfig"></a>__CodeAnalysis__
  | Property 	    | Value                    | Remark                                                    |
  |-------------- |------------------------- | --------------------------------------------------------- |
  | Global Usings | Microsoft.CodeAnalysis   |                                                           |

If you don't want a specific property to be set, you can prevent it. For every property there is a corresponding flag that controls, if the property will be set or not.  
The flags are named BAS_CB_Set_\<PropertyName\>.  
For example if you don't want the NeutralLanguage to be set, add this to your project:

    <PropertyGroup>
      <BAS_CB_Set_NeutralLanguage>false</BAS_CB_Set_NeutralLanguage>
    </PropertyGroup>

### Logging
If you want to log the properties, that are set by this project, you can set the property BAS_CB_Log_Properties to true. This will log the current value of all these properties after the _PrepareForBuild_ target.

    <PropertyGroup>
      <BAS_CB_Log_Properties>true</BAS_CB_Log_Properties>
    </PropertyGroup>

## License
The Basilisque framework (including this repository) is licensed under the [Apache License, Version 2.0](LICENSE.txt).