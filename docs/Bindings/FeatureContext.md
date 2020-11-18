# FeatureContext

SpecFlow provides access to the current test context using both `FeatureContext` and the more commonly used [ScenarioContext](ScenarioContext.md). `FeatureContext` persists for the duration of the execution of an entire feature, whereas `ScenarioContext` only persists for the duration of a scenario.

## Accessing the FeatureContext

### in Bindings

To access the `FeatureContext` you have to get it via [Context-Injection](Context-Injection.md).

Example:  

``` csharp
[Binding]
public class Binding
{
    private FeatureContext _featureContext;

    public Binding(FeatureContext featureContext)
    {
        _featureContext = featureContext;
    }
}

```

Now you can access the FeatureContext in all your Bindings with the `_featureContext` field.

### in Hooks

#### Before/AfterTestRun

Accessing the `FeatureContext` is not possible, as no `Feature` is executed, when the hook is called.

#### Before/AfterFeature

You can get the `FeatureContext` via parameter of the static method.

Example:

``` csharp
[Binding]
public class Hooks
{
    [BeforeFeature]
    public static void BeforeFeature(FeatureContext featureContext)
    {
        Console.WriteLine("Starting " + featureContext.FeatureInfo.Title);
    }

    [AfterFeature]
    public static void AfterFeature(FeatureContext featureContext)
    {
        Console.WriteLine("Finished " + featureContext.FeatureInfo.Title);
    }
}
```

#### Before/AfterScenario

Accessing the `FeatureContext` is done like in [normal bindings](#in-Bindings)

#### Before/AfterStep

Accessing the `FeatureContext` is done like in [normal bindings](#in-Bindings)

## Storing data in the FeatureContext

`FeatureContext` implements Dictionary<string, object>. So you can use the `FeatureContext` like a property bag.  

## Migrating from FeatureContext.Current

With SpecFlow 3.0, we marked FeatureContext.Current as obsolete, to make clear that you that you should avoid using these properties in future. The reason for moving away from these properties is that they do not work when running scenarios in parallel.

So how do you now access FeatureContext?

You can acquire the FeatureContext via [Context-Injection](Context-Injection.md). However, if you want to use it together with Before/After Feature hooks, you need to acquire it via a function parameter.

Previously:

``` csharp
public class Hooks
{
    [AfterFeature()]
    public static void AfterFeature()
    {
        Console.WriteLine("Finished " + FeatureContext.Current.FeatureInfo.Title);
    }
}
```

Now:

``` csharp
public class Hooks
{
    [AfterFeature]
    public static void AfterFeature(FeatureContext featureContext)
    {
        Console.WriteLine("Finished " + featureContext.FeatureInfo.Title);
    }
}
```

## FeatureContext.FeatureInfo

`FeatureInfo` provides more information than `ScenarioInfo`, but it works the same:

In the .feature file:

``` gherkin
Scenario: Showing information of the feature

When I execute any scenario in the feature
Then the FeatureInfo contains the following information
    | Field          | Value                               |
    | Tags           | showUpInScenarioInfo, andThisToo    |
    | Title          | FeatureContext features             |
    | TargetLanguage | CSharp                              |
    | Language       | en-US                               |
    | Description    | In order to                         |
```

and in the step definition:

``` csharp
private class FeatureInformation
{
    public string Title { get; set; }
    public GenerationTargetLanguage TargetLanguage { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }
    public string[] Tags { get; set; }
}

[When(@"I execute any scenario in the feature")]
public void ExecuteAnyScenario() { }

[Then(@"the FeatureInfo contains the following information")]
public void FeatureInfoContainsInterestingInformation(Table table)
{
    // Create our small DTO for the info from the step
    var fromStep =  table.CreateInstance<FeatureInformation>();
    fromStep.Tags = table.Rows[0]["Value"].Split(',');

    var fi = _featureContext.FeatureInfo;

    // Assertions
    fi.Title.Should().Equal(fromStep.Title);
    fi.GenerationTargetLanguage.Should().Equal(fromStep.TargetLanguage);
    fi.Description.Should().StartWith(fromStep.Description);
    fi.Language.IetfLanguageTag.Should().Equal(fromStep.Language);
    for (var i = 0; i < fi.Tags.Length - 1; i++)
    {
        fi.Tags[i].Should().Equal(fromStep.Tags[i]);
    }
}
```

`FeatureContext` exposes a Binding Culture property that simply points to the culture the feature is written in (en-US in our example).
