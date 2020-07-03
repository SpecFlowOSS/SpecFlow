# MSTest

SpecFlow does support MsTest V2. It is not any more working with the old MsTest V1.

## Accessing TestContext

### Using Context Injection

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

### Using the Scenario Container

``` csharp
[Given(@"my test step definition")]
public void MyStepDefinition()
{
   var testContext = ScenarioContext.Current.ScenarioContainer.Resolve<Microsoft.VisualStudio.TestTools.UnitTesting.TestContext>();
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
