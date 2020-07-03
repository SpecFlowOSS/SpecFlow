# xUnit

SpecFlow supports xUnit 2.4 or later.  

Please add the NuGet package [SpecFlow.xUnit](https://www.nuget.org/packages/SpecFlow.xUnit/) to your project to use xUnit in your project.  
You probably also need the xUnit TestAdapter NuGet Package [xunit.runner.visualstudio](https://www.nuget.org/packages/xunit.runner.visualstudio/) in your project that the scenarios will be visible in the Visual Studio Test Explorer.  

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
    public StepsWithScenarioContext(Xunit.Abstractions.ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [When(@"I do something")]
    public void WhenIDoSomething()
    {
        _output.WriteLine("EB7C1291-2C44-417F-ABB7-A5154843BC7B");
    }
}

```
