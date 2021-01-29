# Scoped Step Definitions

Bindings (step definitions, hooks) are global for the entire SpecFlow project. This means that step definitions bound to a very generic step text (e.g. "When I save the changes") become challenging to implement. The general solution for this problem is to phrase the scenario steps in a way that the context is clear (e.g. "When I save the **book details**").

In some cases however, it is necessary to restrict when step definitions or hooks are executed based on certain conditions. SpecFlow's scoped bindings can be used for this purpose.

You can restrict the execution of scoped bindings by:

- tag
- feature (using the feature title)
- scenario (using the scenario title)

The following tags are taken into account for scenario, scenarioblock or step hooks:

- tags defined for the feature
- tags defined for the scenario
- tags defined for the scenario outline
- tags defined for the scenario outline example set (`Examples:`)

_Be careful!_ Coupling your step definitions to features and scenarios is an anti-pattern. [Read more about it on the Cucumber Wiki](https://cucumber.io/docs/guides/anti-patterns/#feature-coupled-step-definitions)

Use the `[Scope]` attribute to define the scope:

```csharp
    [Scope(Tag = "mytag", Feature = "feature title", Scenario = "scenario title")]
```

Navigation from feature files to scoped step definitions is currently not supported by the Visual Studio extension.

## Scoping Rules

Scope can be defined at the method or class level.

If multiple criteria (e.g. both tag and feature) are specified in the same `[Scope]` attribute, they are combined with AND, i.e. all criteria need to match.

_Example AND tag scope decoration:_

```
[Scope(Tag = "thisTag", Feature = "myFeature")]
```

If multiple `[Scope]` attributes are defined for the same method or class, the attributes are combined with OR, i.e. at least one of the `[Scope]` attributes needs to match.

_Examples for OR tag scope decoration:_

```
[Scope(Tag = "thisTag")] [Scope(Tag = "OrThisTag")]
[Scope(Tag = "thisTag"), Scope(Tag = "OrThisTag")]
```

**Note:** Scopes on a different level (class and method) will be combined with OR: defining a `[Scope]` attribute on class level and defining another `[Scope]` at method level will cause the attributes to be combined with OR. If you want an AND combination, use a single Scope, e.g.:

```
[Scope(Feature = "feature title", Scenario = "scenario title")]
```

If a step can be matched to both a step definition without a `[Scope]` attribute as well as a step definition with a `[Scope]` attribute, the step definition with the `[Scope]` attribute is used (no ambiguity).

If a step matches several scoped step definitions, the one with the most restrictions is used. For example, if the first step definition contains `[Scope(Tag = "myTag")]` and the second contains `[Scope(Tag = "myTag", Feature = "myFeature")]` the second step definition (the more specific one) is used if it matches the step.

If you have multiple scoped step definition with the same number of restrictions that match the step, you will get an ambiguous step binding error. For example, if you have a step definition containing `[Scope(Tag = "myTag1", Scenario = "myScenario")]` and another containing `[Scope(Tag = "myTag2", Scenario = "myScenario")]`, you will receive an ambiguous step binding error if the myScenario has **both** the "myTag1" and "myTag2" tags.

## Scope Examples

### Scoped BeforeScenario Hook

The following example starts Selenium for scenarios marked with the `@web` tag.

```c#
[BeforeScenario("web")]
public static void BeforeWebScenario()
{
    StartSelenium();
}
```

### Different Steps for Different Tags

The following example defines a different scope for the same step depending on whether UI automation ("web" tag) or controller automation ("controller" tag) is required:

```c#
    [When(@"I perform a simple search on '(.*)'", Scope(Tag = "controller"))]
    public void WhenIPerformASimpleSearchOn(string searchTerm)
    {
        var controller = new CatalogController();
        actionResult = controller.Search(searchTerm);
    }

    [When(@"I perform a simple search on '(.*)'"), Scope(Tag = "web")]
    public void PerformSimpleSearch(string title)
    {
        selenium.GoToThePage("Home");
        selenium.Type("searchTerm", title);
        selenium.Click("searchButton");
    }
```

## Scoping Tips & Tricks

The following example shows a way to "ignore" executing the scenarios marked with `@manual`. However SpecFlow's tracing will still display the steps, so you can work through the manual scenarios by following the steps in the report.

```c#
    [Binding, Scope(Tag = "manual")]
    public class ManualSteps
    {
        [Given(".*"), When(".*"), Then(".*")]
        public void EmptyStep()
        {
        }

        [Given(".*"), When(".*"), Then(".*")]
        public void EmptyStep(string multiLineStringParam)
        {
        }

        [Given(".*"), When(".*"), Then(".*")]
        public void EmptyStep(Table tableParam)
        {
        }
    }
```

## Beyond Scope

You can define more complex filters using the [ScenarioContext](ScenarioContext.md) class. The following example starts selenium if the scenario is tagged with `@web` _and_ `@automated`.

```c#
    [Binding]
    public class Binding
    {
        ScenarioContext _scenarioContext;

        public Binding(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario("web")]
        public static void BeforeWebScenario()
        {
            if(_scenarioContext.ScenarioInfo.Tags.Contains("automated"))
                StartSelenium();
        }
    }
```
