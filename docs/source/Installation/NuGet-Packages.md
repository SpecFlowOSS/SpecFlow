# NuGet Packages

There are a number of [[NuGet|http://www.nuget.org/]] packages supplied for SpecFlow:

* [`SpecFlow`](http://www.nuget.org/packages/SpecFlow) : The main SpecFlow package. Add this package to your project to install SpecFlow.
* **Unit test provider packages**: These packages are used to configure your unit test provider from SpecFlow 3. You should only install **one** of the packages. Installing more than one package will result in an error.  
  The following packages are available:  
  * `SpecRun.SpecFlow-3.1.0`
  * `SpecFlow.xUnit`
  * `SpecFlow.MsTest`
  * `SpecFlow.NUnit`
* `SpecFlow.Tools.MsBuild.Generation`: This package generates the code-behind files required by SpecFlow using MSBuild. This package is not required prior to SpecFlow 3, but we **strongly recommend** using MSBuild to generate your code behind files with all versions of SpecFlow!


**Note:** The `SpecFlow` NuGet package only contains SpecFlow's generator and the runtime components. You still need to [[install|installation]] the IDE integration.

The easiest way to add these packages to your project is to right-click your project and select **Manage NuGet Packages**. You can add SpecFlow to your project with the NuGet Package Management Console with
```
Install-Package SpecFlow -ProjectName myproject
```

The [[SpecFlow.CustomPlugin|http://www.nuget.org/packages/SpecFlow.CustomPlugin]] NuGet package can be used to implement custom [[plugins]] for SpecFlow.