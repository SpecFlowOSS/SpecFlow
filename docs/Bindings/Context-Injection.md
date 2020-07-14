# Context Injection

SpecFlow supports a very simple dependency framework that is able to instantiate and inject class instances for scenarios. This feature allows you to group the shared state in context classes, and inject them into every binding class that needs access to that shared state.

To use context injection:

1. Create your POCOs (simple .NET classes) representing the shared data.
2. Define them as constructor parameters in every binding class that requires them.
3. Save the constructor argument to instance fields, so you can use them in the step definitions.

Rules:

* The life-time of these objects is limited to a scenario's execution. 
* If the injected objects implement `IDisposable`, they will be disposed after the scenario is executed.
* The injection is resolved recursively, i.e. the injected class can also have dependencies. 
* Resolution is done using public constructors only. 
* If there are multiple public constructors, SpecFlow takes the first one.

The container used by SpecFlow can be customized, e.g. you can include object instances that have already been created, or modify the resolution rules. See the _Advanced options_ section below for details.

## Examples

In the first example we define a POCO for holding the data of a person and use it in a _given_ and a _then_ step that are placed in different binding classes.

```c#
public class PersonData // the POCO for sharing person data
{ 
  public string FirstName;
  public string LastName;
}

[Binding]
public class MyStepDefs
{
  private readonly PersonData personData;
  public MyStepDefs(PersonData personData) // use it as ctor parameter
  { 
    this.personData = personData;
  }
  
  [Given] 
  public void The_person_FIRSTNAME_LASTNAME(string firstName, string lastName) 
  {
    personData.FirstName = firstName; // write into the shared data
    personData.LastName = lastName;
    //... do other things you need
  }
}

[Binding]
public class OtherStepDefs // another binding class needing the person
{ 
  private readonly PersonData personData;
  public OtherStepDefs(PersonData personData) // ctor parameter here too
  { 
    this.personData = personData;
  }
  
  [Then] 
  public void The_person_data_is_properly_displayed() 
  {
    var displayedData = ... // get the displayed data from the app
      // read from shared data, to perform assertions
    Assert.AreEqual(personData.FirstName + " " + personData.LastName, 
      displayedData, "Person name was not displayed properly");
  }
}
```

The following example defines a context class to store referred books. The context class is injected into a binding class.

```c#
public class CatalogContext
{
    public CatalogContext()
    {
        ReferenceBooks = new ReferenceBookList();
    }

    public ReferenceBookList ReferenceBooks { get; set; }
}

[Binding]
public class BookSteps
{
    private readonly CatalogContext _catalogContext;

    public BookSteps(CatalogContext catalogContext)
    {
        _catalogContext = catalogContext;
    }

    [Given(@"the following books")]
    public void GivenTheFollowingBooks(Table table)
    {
        foreach (var book in table.CreateSet<Book>())
        {
            SaveBook(book);
            _catalogContext.ReferenceBooks.Add(book.Id, book);
        }
    }
}
```

## Advanced options

The container used by SpecFlow can be customized, e.g. you can include object instances that have already been created, or modify the resolution rules. 

You can customize the container from a [plugin](../Extend/Plugins.md) or a before scenario [hook](Hooks.md). The class customizing the injection rules has to obtain an instance of the scenario execution container (an instance of `BoDi.IObjectContainer`). This can be done through constructor injection (see example below).

The following example adds the Selenium web driver to the container, so that binding classes can specify `IWebDriver` dependencies (a constructor argument of type `IWebDriver`).

```c#
[Binding]
public class WebDriverSupport
{
  private readonly IObjectContainer objectContainer;

  public WebDriverSupport(IObjectContainer objectContainer)
  {
    this.objectContainer = objectContainer;
  }

  [BeforeScenario]
  public void InitializeWebDriver()
  {
    var webDriver = new FirefoxDriver();
    objectContainer.RegisterInstanceAs<IWebDriver>(webDriver);
  }
}
```

## Custom Dependency Injection Frameworks

As mentioned above, the default SpecFlow container is `IObjectContainer` which is recommended for most scenarios. However, you may have situations where you need more control over the configuration of the dependency injection, or make use of an existing dependency injection configuration within the project you are testing, e.g. pulling in service layers for assisting with assertions in `Then` stages.

### Consuming existing plugins
- [SpecFlow.Autofac](https://github.com/gasparnagy/SpecFlow.Autofac)
- [SpecFlow.Unity](https://github.com/phatcher/SpecFlow.Unity)
- [SpecFlow.Ninject](https://github.com/MattMcKinney/SpecFlow.Ninject) (currently not on nuget)

To make use of these plugins, you need to add a reference and add the plugin to your configuration in the `specflow` section:

```xml
<specFlow>
  <plugins>
    <add name="SpecFlow.Autofac" type="Runtime" />
  </plugins>
  <!-- Anything else -->
</specFlow>
```

This tells SpecFlow to load the runtime plugin and allows you to create an entry point to use this functionality, as [shown in the autofac example](https://github.com/gasparnagy/SpecFlow.Autofac/blob/master/sample/MyCalculator/MyCalculator.Specs/Support/TestDependencies.cs). Once set up, your dependencies are injected into steps and bindings like they are with the `IObjectContainer`, but behind the scenes it will be pulling those dependencies from the DI container you added.

> One thing to note here is that each plugin has its own conventions for loading the entry point. This is often a static class with a static method containing an attribute that is marked by the specific plugin. You should check the requirements of the plugins you are using.

You can load all your dependencies within this handler section, or you can to inject the relevant IoC container into your binding sections like this:

```csharp
[Binding]
public class WebDriverPageHooks
{
    private readonly IKernel _kernel;

    // Inject in our container (using Ninject here)
    public WebDriverPageHooks(IKernel kernel)
    { _kernel = kernel; }
    
    private IWebDriver SetupWebDriver()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--disable-notifications");
        return new ChromeDriver(options);
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
        var webdriver = SetupWebDriver();        
        _kernel.Bind<IWebDriver>().ToConstant(webdriver);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        var webDriver = _kernel.Get<IWebDriver>();
        
        // Output any screenshots or log dumps etc
        
        webDriver.Close();
        webDriver.Dispose();
    }
}
```

This gives you the option of either loading types up front or creating types within your binding sections so you can dispose of them as necessary.

### Creating your own

We recommend looking at the autofac example and [plugins documentation](https://specflow.org/documentation/Plugins/) and following these conventions.

> Remember to adhere to the plugin documentation and have your assembly end in `.SpecFlowPlugin` e.g. `SpecFlow.AutoFac.SpecFlowPlugin`. Internal namespaces can be anything you want, but the assembly name must follow this naming convention or SpecFlow will be unable to locate it.