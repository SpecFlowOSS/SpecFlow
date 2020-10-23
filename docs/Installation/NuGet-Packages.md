# NuGet Packages

There are a number of [NuGet](http://www.nuget.org/) packages supplied for SpecFlow:

* [`SpecFlow`](http://www.nuget.org/packages/SpecFlow) : The main SpecFlow package. Add this package to your project to install SpecFlow.
* **Unit test provider packages**: These packages are used to configure your unit test provider from SpecFlow 3. You should only install **one** of the packages. Installing more than one package will result in an error.  
  The following packages are available:  
  * `SpecRun.SpecFlow-3.3.0`
  * `SpecFlow.xUnit`
  * `SpecFlow.MsTest`
  * `SpecFlow.NUnit`
* `SpecFlow.Tools.MsBuild.Generation`: This package generates the code-behind files required by SpecFlow using MSBuild. This package is not required prior to SpecFlow 3, but we **strongly recommend** using MSBuild to generate your code behind files with all versions of SpecFlow!


**Note:** The `SpecFlow` NuGet package only contains SpecFlow's generator and the runtime components. You still need to [install](Installation.md) the IDE integration.

The easiest way to add these packages to your project is to right-click your project and select **Manage NuGet Packages**. You can add SpecFlow to your project with the NuGet Package Management Console with
```
Install-Package SpecFlow -ProjectName myproject
```

The [SpecFlow.CustomPlugin](https://www.nuget.org/packages/SpecFlow.CustomPlugin) NuGet package can be used to implement custom [plugins](../Extend/Plugins.md) for SpecFlow.


## NuGet packages after 3.0

### SpecFlow

<https://www.nuget.org/packages/SpecFlow/>

This is the main package of SpecFlow and contains all parts needed at Runtime.

### SpecFlow.Tools.MsBuild.Generation

<https://www.nuget.org/packages/SpecFlow.Tools.MsBuild.Generation/>

This package enables the code-behind file generation at build time.  

#### >= 3.0

It is **mandatory** for projects to use. After SpecFlow 3.3.30 this is a dependency of the `SpecFlow.xUnit`, `SpecFlow.NUnit`, `SpecFlow.MSTest` and `SpecRun.SpecFlow.3-3-0` packages, hence the package is automatically installed with the unit test provider packages and you don't have to install it manually.

#### < 3.0

This package is optional if the code-behind file generation is enabled in the Visual Studio Extension. However, we recommend to upgrade to the [MSBuild code behind file generation](../Tools/Generate-Tests-From-MsBuild.md).

### SpecFlow.xUnit

<https://www.nuget.org/packages/SpecFlow.xUnit/>

#### >= 3.0

If you want to use SpecFlow with xUnit, you have to use this packages, as it does the configuration for this.  

We don't support older versions than xUnit 2.4.0.

#### < 3.0

This package is optional to use, as all steps can be done manually.
It changes automatically the `app.config` to use xUnit for you and has a dependency on xUnit (>= 2.0).

### SpecFlow.MsTest

<https://www.nuget.org/packages/SpecFlow.MsTest/>

#### >= 3.0

If you want to use SpecFlow with MsTest V2, you have to use this packages, as it does the configuration for this.  

We don't support older versions than MsTest V2 1.3.2.

#### < 3.0

This package is optional to use, as all steps can be done manually.
It changes automatically the `app.config` to use MsTest. No additional dependencies are added.

We support MsTest V1 and V2.

### SpecFlow.NUnit

<https://www.nuget.org/packages/SpecFlow.NUnit/>

#### > 3.0

If you want to use SpecFlow with NUnit, you have to use this packages, as it does the configuration for this.  

We don't support older versions than NUnit 3.11.0.

#### < 3.0

This package is optional to use, as all steps can be done manually.
It changes automatically the `app.config` to use NUnit and has a dependency on NUnit (>= 3.0).
If you want to use earlier version of NUnit, you have to do the changes manually.

We support NUnit 2 & NUnit 3.

### SpecFlow.NUnit.Runners

<https://www.nuget.org/packages/SpecFlow.NUnit.Runners/>

This is a meta-package to install the NUnit.Console package additionally.

### SpecFlow.CustomPlugin

<https://www.nuget.org/packages/SpecFlow.CustomPlugin/>

This package is for writing your own runtime or generator plugins.

