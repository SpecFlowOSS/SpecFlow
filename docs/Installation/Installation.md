# Installation

_Note:_ If you are new to SpecFlow, we recommend checking out the [Getting Started guide](https://go.specflow.org/getting-started) first. It will take you through the process of installing SpecFlow and setting up your first project and tests in Visual Studio.

SpecFlow consists of three components:

* The **IDE Integration** that provides a customized editor and test generation functions within your IDE.
* The **generator** that can turn Gherkin specifications into executable test classes, available from NuGet.
* The **runtime** required for executing the generated tests. There are different runtime assemblies compiled for different target platforms. These packages are also available from NuGet.

In order to install everything you need, you first have to install the IDE integration and then set up your project to work with SpecFlow using the NuGet packages.

## Installing the IDE Integration

The process of installing the IDE Integration packages depends on your IDE.

### Visual Studio

We recommend installing the SpecFlow Visual Studio extension (IDE Integration), as this is the most convenient way of working with SpecFlow. An overview of the features provided by the integration can be found [here](../Tools/Visual-Studio-Integration.md).

**If you are using Deveroom, do not install the SpecFlow Visual Studio extension; you should only install one of these 2 extensions.**

The easiest way to install the IDE integration is to select **Tools\Extensions and Updates** from the menu and search for "SpecFlow" in the online gallery.  

The integration packages can also be downloaded and installed separately from the Visual Studio Gallery:  

* [VS2019 integration](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowForVisualStudio)
* [VS2017 integration](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowforVisualStudio2017)
* [VS2015 integration](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowforVisualStudio2015)

### MonoDevelop/XamarinStudio/Visual Studio for Mac

We don't maintain our own extension for MonoDevelop/XamarinStudio/Visual Studio for Mac. But our amazing community created on at <https://github.com/straighteight/SpecFlow-VS-Mac-Integration>.

### VSCode

We currently don't have our own extension for VSCode.

For creating new projects, we recommend to use our [.NET templates](../Installation/Project-and-Item-Templates.html#creating-a-new-project-from-the-template).

To improve your developing and scenario writing experience, we recommend following VSCode extensions:

* [Cucumber (Gherkin) Full Support](https://marketplace.visualstudio.com/items?itemName=alexkrechik.cucumberautocomplete) - for editing feature files
* [.NET Core Test Explore](https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer) - for executing scenarios

### Rider

We currently don't have our own extension for Rider.

For creating new projects, we recommend to use our [.NET templates](../Installation/Project-and-Item-Templates.html#creating-a-new-project-from-the-template).

If you are interested in one, please up-vote this [feature-request](https://support.specflow.org/hc/en-us/community/posts/360012011397--SpecFlow-Rider-IDE-Support)

## Project Setup

The generator and runtime are usually installed together for each project. To install the NuGet packages:

1. Right-click on your project in Visual Studio, and select **Manage NuGet Packages** from the menu.
1. Switch to the **Browse** tab.
1. Enter "SpecFlow" in the search field to list the available packages for SpecFlow.
1. Install the required NuGet packages. Depending on your chosen unit test provider, you have to use different packages. See [this list](Unit-Test-Providers.md) to find the correct package
