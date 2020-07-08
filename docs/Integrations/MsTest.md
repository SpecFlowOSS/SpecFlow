# MSTest

SpecFlow does support MsTest V2. It is not any more working with the old MsTest V1.

Documentation for MSTest can be found [here](https://docs.microsoft.com/en-us/visualstudio/test/unit-test-your-code?view=vs-2019).

## Needed NuGet Packages

For SpecFlow: [SpecFlow.MSTest](https://www.nuget.org/packages/SpecFlow.MSTest/)  

For MSTest: [MSTest.TestFramework](https://www.nuget.org/packages/MSTest.TestFramework/)  

For Test Discovery & Execution:

- [MSTest.TestAdapter](https://www.nuget.org/packages/MSTest.TestAdapter/)
- [Microsoft.NET.Test.Sdk](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk)

## Accessing TestContext

``` csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class MyStepDefs
{
    private readonly TestContext _testContext;
    public MyStepDefs(TestContext testContext) // use it as ctor parameter
    { 
        _testContext = testContext;
    }

    [BeforeScenario()]
    public void BeforeScenario()
    {
        //now you can access the TestContext
    } 
}
```

## Tags for TestClass Attributes

The MsTest Generator can generate test class attributes from tags specified on a **feature**.

### Owner

Tag:

``` gherkin
@Owner:John
```

Output:

``` csharp
[Microsoft.VisualStudio.TestTools.UnitTesting.OwnerAttribute("John")]
```

### WorkItem

Tag:

``` gherkin
@WorkItem:123
```

Output:

``` csharp
[Microsoft.VisualStudio.TestTools.UnitTesting.WorkItemAttribute(123)]
```

### DeploymentItem

#### Example 1 : Copy a file to the same directory as the deployed test assemblies

Tag:

``` gherkin
@MsTest:DeploymentItem:test.txt
```

Output:

``` csharp
[Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute("test.txt")]
```

#### Example 2 : Copy a file to a sub-directory relative to the deployment directory

Tag:

``` gherkin
@MsTest:DeploymentItem:Resources\DeploymentItemTestFile.txt:Data
```

Output:

``` csharp
[Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute("Resources\\DeploymentItemTestFile.txt", "Data")]
```
