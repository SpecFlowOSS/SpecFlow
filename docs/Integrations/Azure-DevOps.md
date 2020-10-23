# Azure DevOps (Server)

## Build Integration

The easiest way to execute SpecFlow scenarios on Azure DevOps (Team Foundation Server (TFS) Build is to use [SpecFlow+ Runner](http://www.specflow.org/plus/runner) as unit test provider (see  [SpecFlow+ Runner Integration]). The SpecRun NuGet package contains all necessary integration components, and you don't need to do any additional configuration or build process template modification to let TFS build execute your scenarios, and even more:

* Display scenario titles in the execution result
* Generate detailed and customizable HTML report
* Allows filtering scenarios in the TFS build definition
* The integration also works with the hosted Azure DevOps Server ( [http://tfs.visualstudio.com/])

### Legacy Integration

As SpecFlow generates unit test code from the scenarios, the tests can be executed on any build server (as unit tests). The configuration depends on the unit test provider used.
