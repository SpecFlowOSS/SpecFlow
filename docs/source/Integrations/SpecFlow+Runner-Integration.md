# SpecFlow+ Runner

[SpecFlow+ Runner](http://specflow.org/plus/runner/) (formerly "SpecRun") is a dedicated test execution framework for SpecFlow. SpecFlow+ Runner integrates more tightly with Visual Studio's testing infrastructure and Team Foundation Server (TFS) Build. The documentation for SpecFlow+ can be found [here](http://specflow.org/plus/documentation/).

## Installation

SpecFlow+ Runner is provided as a NuGet package ([SpecRun.SpecFlow](http://www.nuget.org/packages/SpecRun.SpecFlow]). Detailed setup instructions can be found [here](https://specflow.org/plus/documentation/SpecFlow--Runner-Installation/).  

## Visual Studio Test Explorer Support

SpecFlow+ Runner allows you to run and debug your scenarios as first class citizens:

* Run/debug individual scenarios or scenario outline examples from the feature file editor (choose "Run/Debug SpecFlow Scenarios" from the context menu)
* View scenarios in the Visual Studio Test Explorer window with the scenario title
* Use the Test Explorer to:
  * Group scenarios by tags (choose "Traits" grouping) and features (choose "Class")
  * Filter scenarios by different criteria
  * Run/debug selected/all scenarios
  * Jump to the corresponding scenario in the feature file
  * View test execution results
* You can specify [processor architecture (x86/x64), .NET platform and many other details for the test execution](https://specflow.org/plus/documentation/Environment/), including special [config file transformations](https://specflow.org/plus/documentation/DeploymentTransformation/) used for the test execution only. 

## Team Foundation Server Support

The SpecRun NuGet package contains all necessary integration components for Team Foundation Server Build, and you do not need to make any additional configuration or build process template modifications for TFS Build to execute your scenarios. You can also:

* Display scenario titles in the execution result
* Generate detailed and customizable HTML report
* Filter scenarios in the TFS build definition

More information on using SpecFlow+ Runner with build servers can be found [here](https://specflow.org/plus/documentation/SpecFlowPlus-and-Build-Servers/).

## Test Execution Features

SpecFlow+ Runner is a smarter integration test runner for SpecFlow: 

* Faster feedback: parallel test execution and smart execution order
* More information: advanced metrics, detecting random failures, historical execution statistics
* Not limited to SpecFlow: execute integration tests written with other unit testing frameworks

See the short introduction video about the [configurable test execution environments](http://go.specflow.org/specrun-testenv) and about [parallel test execution](http://go.specflow.org/specrun-parallel).