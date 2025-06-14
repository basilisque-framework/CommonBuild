<!--
   Copyright 2025 Alexander Stärk

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
[![GitHub](https://img.shields.io/badge/GitHub-Project-%23004880.svg?logo=github)](https://github.com/basilisque-framework/CommonBuild)
[![License](https://img.shields.io/badge/License-Apache%20License%202.0-%23D22128.svg?logo=apache&logoColor=%23D22128)](https://github.com/basilisque-framework/CommonBuild/blob/main/LICENSE.txt)  

This project provides __optional__ common build configuration for projects that use the Basilisque framework. But it also can be used without Basilique.  
So if you like the contained configuration feel free to use it, otherwise simply don't.

## Description
This project doesn't contain any assemblies that will be deployed with the target project. Instead it only contains build time configuration (.props/.targets-files, ...) to provide a common basic set of configuration to all target projects.

### Usage
Simply installing this NuGet package will automatically include the contained .props and .targets files in the build.
For more information about how this can be configured, please see the [GitHub page](https://github.com/basilisque-framework/CommonBuild).

### Conventions
The configuration is based on conventions regarding the names of the target projects. _(The folder structure of the solution is irrelevant.)_

>__ExampleSolution__
>- MyProject.__Service__  
>_Containing the startup code of a backend (Windows) service_
>- MyProject.Service.__Tests__  
>_Containing automated tests. In this case for the MyProject.Service-project_
>- MyProject.__API__  
>_Containing the public facing code of the backend (API-Controllers, ...)_
>- MyProject.API.__Tests__  
>_Containing automated tests. In this case for the MyProject.API-project_
>- MyProject.__Domain__  
>_Containing the business logic_
>- MyProject.Domain.__Tests__  
>_Containing automated tests. In this case for the MyProject.Domain-project_
>- MyProject.__DataAccess__  
>_Containing the data access layer (e.g. using Entity Framework)_
>- MyProject.DataAccess.__Tests__  
>_Containing automated tests. In this case for the MyProject.DataAccess-project_
>- MyProject.__Benchmarks__  
>_Containing benchmarks for a project_
>- MyProject.__CodeAnalysis__  
>_Containing code analysis support for the project like source generators, analyzers and fixes_

Obviously not all applications need all of those project types. So e.g. if you don't need data access, then just do not create a project named like it. But if you do need data access, then let the project name end with _.DataAccess_. That is the intention behind the configuration in Basilisque.CommonBuild.

### Provided Configuration
The package provides many different default configurations and versioning.
For detailed information, please have a look at the GitHub page:  
[![GitHub](https://img.shields.io/badge/GitHub-Project-%23004880.svg?logo=github)](https://github.com/basilisque-framework/CommonBuild)

## License
The Basilisque framework (including this repository) is licensed under the [Apache License, Version 2.0](https://github.com/basilisque-framework/CommonBuild/blob/main/LICENSE.txt).