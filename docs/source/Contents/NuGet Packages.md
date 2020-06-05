# Description of usage and function of the different SpecFlow NuGet packages

With SpecFlow 3.0 some NuGet packages got additional functionality and are no more optional to install.

## NuGet packages after 3.0

### SpecFlow

<https://www.nuget.org/packages/SpecFlow/>

This is the main package of SpecFlow and contains all parts needed at Runtime.

### SpecFlow.Tools.MsBuild.Generation

<https://www.nuget.org/packages/SpecFlow.Tools.MsBuild.Generation/>

This package enables the code-behind file generation at build time.  

#### >= 3.0

It is **mandatory** for projects in the SDK- Style format.

#### < 3.0

This package is optional, because the code- behind file generation is done in the most cases by Visual Studio. But it is highly recommendated to use it.

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

