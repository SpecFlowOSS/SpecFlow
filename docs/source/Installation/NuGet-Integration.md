_Editor note: We recommend reading this documentation entry at [[http://www.specflow.org/documentation/NuGet-Integration]]. We use the GitHub wiki for authoring the documentation pages._

SpecFlow is available as a [[NuGet|http://www.nuget.org/]] package: [[SpecFlow|http://www.nuget.org/packages/SpecFlow]]. Add this package to your project to install SpecFlow

**Note:** The NuGet package only contains the generator and the runtime components of SpecFlow. You still need to install the IDE integration, e.g. from [[Visual Studio Gallery|http://go.specflow.org/vsgallery]] (see details at [[Installation]]).

You can add SpecFlow to your project with the NuGet Package Management Console with
```
Install-Package SpecFlow -ProjectName myproject
```

For setting up SpecFlow with specific unit test providers in one step, there are two additional packages

* [[SpecFlow.NUnit|http://www.nuget.org/packages/SpecFlow.NUnit]] - installs SpecFlow and NUnit for running with arbitrary test runners (e.g. ReSharper)
* [[SpecFlow.NUnit.Runners|http://www.nuget.org/packages/SpecFlow.NUnit.Runners]] - this is a specialized package, if you want to run SpecFlow tests with the nunit test runners together with `[AfterTestRun]` hooks. (see https://github.com/techtalk/SpecFlow/issues/26)
* [[SpecFlow.xUnit|http://www.nuget.org/packages/SpecFlow.xUnit]] - installs SpecFlow and xUnit
* [[SpecRun.SpecFlow|http://www.nuget.org/packages/SpecRun.SpecFlow]] - installs SpecFlow and [[SpecRun|http://www.specrun.com]]

For implementing custom [[plugins]] for SpecFlow, the [[SpecFlow.CustomPlugin|http://www.nuget.org/packages/SpecFlow.CustomPlugin]] NuGet package can be used.