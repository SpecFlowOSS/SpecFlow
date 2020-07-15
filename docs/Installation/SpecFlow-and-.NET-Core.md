# .NET Core

**Note:** Support for .NET Core is only available from SpecFlow 3 onwards.

Setting up SpecFlow to work with .NET Core projects in SpecFlow 3 is different from configuring projects using .NET Framework in previous versions of SpecFlow. 

For details on the general configuration of SpecFlow, see [[Configuration]].

## Prerequisites

Microsoft.NET.Test.Sdk 15 or higher is required. If you have not installed the SDK, please do so.

## Ensure Your Visual Studio Extension is Up-to-date

The SpecFlow Visual Studio with support for .NET Core is not compatible with versions of SpecFlow prior to 2.3.2. If you have disabled automatic updates for the extension, please enable the automatic updates again to upgrade to the newest version.

To do so:

1. Select Tools | Extensions and Updates from the menu in Visual Studio.
1. Enter “SpecFlow’ in the search field to restrict the entries in the list.
1. Click on the “SpecFlow for Visual Studio” entry and enable the **Automatically update this extension** check box.  
  ![Automatically update this extension](https://specflow.org/wp-content/uploads/2018/10/Disable-Extension-Updates-e1540466494951.png?_t=1540466495)
1. This will prevent newer versions of the extension from being installed automatically. Once you are ready to upgrade to SpecFlow 3, you can enable this option again.

## Configuring the Unit Test Provider

The configuration is no longer in your app.config file. Instead, the unit test provider is now configured using plugins for the desired test frameworks. You will therefore need to add **one** of the following NuGet packages to your project to configure the unit test provider:

* SpecRun.SpecFlow-3.3.0
* SpecFlow.xUnit
* SpecFlow.MsTest
* SpecFlow.NUnit

**Note:** Make sure you do not add more than one of the unit test plugins to your project. If you do, an error message will be displayed.

To add the required packages:
<ol>
<li>Right-click on your project, and select <strong>Manage NuGet Packages</strong>.</li>
<li>Enable the <strong>Include prerelease </strong>option and search for “SpecFlow”.</li>
<li>Install/update the following packages as required:
<ul>
<li>SpecFlow</li>
<li><strong>One</strong> of the unit test provider packages (see above).</li>
</ul>
</li>
<li>Install the test runner for your unit test provider (e.g. <em>xunit.runner.visualstudio</em>).</li>
</ol>

## Configuration Options

When using .NET Core, [configuration](Configuration.md) options must be configured in the new `specflow.json` configuration file. This file is optional when using the Full Framework.

The structure of the .json configuration file reflects the structure of the old app.config. Some examples can be found [here](https://github.com/techtalk/SpecFlow-Examples/tree/feature/netcore-examples/SpecFlow.json).

**Example:** The following sets the feature file language is set to “de-AT”.

``` json
{
    "language":
    {
        "feature": "de-AT"
    }
}
```

For more details, refer to the [configuration](Configuration.md) section.

## Generating Code-behind Files with MSBuild

You need to use the SpecFlow.Tools.MsBuild.Generation NuGet package to generate the code-behind files, see [here](../Tools/Generate-Tests-From-MsBuild.md).

## Sample Projects

.NET Core versions of the example projects are located [here](https://github.com/SpecFlowOSS/SpecFlow-Examples/tree/feature/netcore-examples/).
