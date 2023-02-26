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
This repository provides __optional__ common build configuration for projects that use the Basilisque framework. But it also can be used without Basilique.  
So if you like the contained configuration feel free to use it, otherwise simply don't.

## Description
This project doesn't contain any assemblies that will be deployed with the target project. Instead it only contains build time configuration (.props/.targets-files) to provide a common basic set of configuration to all target projects.

### Usage
Install the [NuGet package](https://www.nuget.org/packages/Basilisque.CommonBuild)  
This will automatically include the contained .props and .targets files in the build.

### Conventions
The configuration is based on conventions regarding the names of the target projects. _(The folder structure of the solution is irrelevant.)_

>__ExampleSolution__
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
Obviously not all applications need all of those project types. So e.g. if you don't need data access, then just do not create a project named like it. But if you do need data access, then let the project name end with _.DataAccess_. That is the intention behind the configuration in this repository.

### Provided Configuration
__.props / .targets__  
<!--
- <a name="servicesConfig"></a>Service
   - ???
- <a name="apiConfig"></a>API
   - ???
- <a name="domainConfig"></a>Domain
   - ???
- <a name="dataAccessConfig"></a>DataAccess
   - ???
- <a name="testsConfig"></a>Tests
   - ???
-->
## License
The Basilisque framework (including this repository) is licensed under the [Apache License, Version 2.0](LICENSE.txt).