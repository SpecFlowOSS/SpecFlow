# Project and Item Templates

**Note:** The [.NET Core SDK|https://dotnet.microsoft.com/download]() must be installed to use project templates.

## Installing the Project Template

To install the project template:

1. Open the command line interface of your choice (e.g. cmd or powershell).
1. Execute the following command:  
  `dotnet new -i SpecFlow.Templates.DotNet`
1. The template is installed locally. Once the installation is complete, you can use the template to create new SpecFlow projects.

## Creating a New Project from the Template

After installing the templates, create a new project using the following command:

`dotnet new specflowproject`

By default, a .NET Core project is created with SpecFlow+ Runner configured as the test runner. You can create a project for a different test runner and/or target framework using the following optional parameters:

* `framework`: Determine the target framework for the project. The following options are available:
  *  `netcoreapp3.0` (default): .NET Core 3.0
  *  `netcoreapp3.1` (default): if .NET Core 3.1 is installed (presence of Core 3.x is mutual exklusive)
  *  `netcoreapp2.2`: .NET Core 2.2
  *  `net472`: .NET Framework 4.72
* `unittestprovider`: Determines the test runner. The following options are available:
  * `specflowplusrunner` (default): SpecFlow+ Runner
  * `xunit`: XUnit
  * `nunit`: NUnit
  * `mstest`: MSTest

**Example:**
`dotnet new specflowproject --unittestprovider xunit --framework netcoreapp2.2`

This creates a new project with XUnit as the unit test provider, and targetting .NET Core 2.2. The project is created with a pre-defined structure that follows best practices. The project includes a single feature file (in the `Features` folder) and its associated steps (in the `Steps` folder).

## Item Templates

The template pack also includes a few item templates:

* `specflow-feature`: .feature file in English
* `specflow-json`: specflow.json configuration file
* `specflow-plus-profile`: Default.srProfile (SpecFlow+Runner configuration)

If you have additional ideas for the templates, please open a GitHub issue <a href="https://github.com/techtalk/SpecFlow/issues">here</a>.
