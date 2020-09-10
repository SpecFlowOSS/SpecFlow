# xUnit

SpecFlow supports xUnit 2.4 or later.  

Documentation for xUnit can be found [here](https://xunit.net/#documentation).

## Needed NuGet Packages

For SpecFlow: [SpecFlow.xUnit](https://www.nuget.org/packages/SpecFlow.xUnit/)

For xUnit: [xUnit](https://www.nuget.org/packages/xunit/)  

For Test Discovery & Execution:

- [xunit.runner.visualstudio](https://www.nuget.org/packages/xunit.runner.visualstudio/)
- [Microsoft.NET.Test.Sdk](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk)

## Access ITestOutputHelper

The xUnit ITestOutputHelper is registered in the ScenarioContainer. You can get access to simply via getting it via [Context-Injection](../Bindings/Context-Injection.md).

### Example

``` csharp

using System;
using TechTalk.SpecFlow;

[Binding]
public class BindingClass
{
    private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
    public BindingClass(Xunit.Abstractions.ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [When(@"I do something")]
    public void WhenIDoSomething()
    {
        _testOutputHelper.WriteLine("EB7C1291-2C44-417F-ABB7-A5154843BC7B");
    }
}

```
