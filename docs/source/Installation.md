# Installation

_Note:_ If you are new to SpecFlow, we recommend checking out the [[Getting Started guide|http://go.specflow.org/getting-started]] first. It will take you through the process of installing SpecFlow and setting up your first project and tests in Visual Studio. 

SpecFlow consists of three components:

* The **IDE Integration** that provides a customized editor and test generation functions within your IDE. This is provided as an extension for Visual Studio.
* The **generator** that can turn Gherkin specifications into executable test classes, available from NuGet.
* The **runtime** required for executing the generated tests. There are different runtime assemblies compiled for different target platforms. These packages are also available from NuGet.

In order to install everything you need, you first have to install the IDE integration and then set up your project to work with SpecFlow using the NuGet packages.

## Installing the IDE Integration

The process of installing the IDE Integration packages depends on your IDE. If you are using Visual Studio, the easiest way is to search for “SpecFlow” in the online search in the extension manager (**Tools | Extensions and Updates**). For other IDE integrations and for the direct download links, see the [[Install IDE Integration]] page.

This step only needs to be performed once in Visual Studio.

## Project Setup

The generator and runtime are usually installed together for each project. To install the NuGet packages:

1. Right-click on your project in Visual Studio, and select **Manage NuGet Packages** from the menu.
1. Switch to the **Browse** tab.
1. Enter "SpecFlow" in the search field to list the available packages for SpecFlow.
1. Install the required packages. You need to install the 
SpecFlow NuGet package (`[[SpecFlow|http://www.nuget.org/packages/SpecFlow]]`). 
Other helper packages are also availble, e.g. `[[SpecFlow.NUnit|http://www.nuget.org/packages/SpecFlow.NUnit]]` or `[[SpecRun.SpecFlow|http://www.nuget.org/packages/SpecRun.SpecFlow]]`. We recommend that you install the NuGet packages for your test framework.

You can also install NuGet package manager console, e.g.:

```
Install-Package SpecFlow -ProjectName MyApp.Specs
```

Refer to the [[NuGet Integration]] page a full list of supported NuGet packages. You can find more details on setting up your project on the [[Setup SpecFlow Projects]] page.