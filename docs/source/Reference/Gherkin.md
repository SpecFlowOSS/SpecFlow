# Using the Gherkin Language in SpecFlow

The feature files used by SpecFlow to specify acceptance criteria for features (use cases, user stories) in your application are defined using the Gherkin syntax. The Gherkin language defines the structure and a basic syntax for describing tests. The Gherkin format was introduced by [[Cucumber|http://cukes.info/]] and is also used by other tools. 

The Gherkin language is maintained as a separate project [[on GitHub|https://github.com/cucumber/cucumber/tree/master/gherkin]]. A detailed description of the language can be found [[here|https://cucumber.io/docs/gherkin/reference/]].

This page focuses on how SpecFlow handles the different Gherkin language elements. 

## Features
The feature element provides a header for the feature file. The feature element includes the name and a high level description of the corresponding feature in your application. More details can be found [[here|https://cucumber.io/docs/gherkin/reference/#feature]].

SpecFlow generates a unit test class for the feature element, with the class name derived from the name of the feature.

## Scenarios
A feature file may contain multiple scenarios used to describe the feature's acceptance tests. Scenarios have a name and can consist of multiple scenario steps. More details can be found [[here|https://cucumber.io/docs/gherkin/reference/#example]].

SpecFlow generates a unit test method for each scenario, with the method name derived from the name of the scenario.

## Scenario Steps
Scenarios can contain multiple scenario steps. There are three types of steps that define either the preconditions, actions or verification steps that make up the acceptance test (these three types are also referred to as arrange, act and assert). The different types of steps begin with either the `Given`, `When` or `Then` keywords respectively (in English feature files), and subsequent steps of the same type can be linked using the `And` and `But` keywords. There may be other alternative keywords for specifying steps.

The Gherkin syntax allows any combination of these three types of steps, but a scenario usually has distinct blocks of `Given`, `When` and `Then` statements.

Scenario steps are defined using text and can have additional table (called `DataTable`) or multi-line text (called `DocString`) arguments.

More details can be found [[here|https://cucumber.io/docs/gherkin/reference/#steps]].

SpecFlow generates a call inside the unit test method for each scenario step. The call is performed by the SpecFlow runtime that will execute the step definition matching to the scenario step. The matching is done at runtime, so the generated tests can be compiled and executed even if the binding is not yet implemented. Read more about execution of test before the binding has been implemented: [[Missing, Pending or Improperly Configured Bindings|Test Result]].

The scenario steps are primary way to execute any custom code to automate the application. You can read more about the different bindings in the [[Bindings]] page.

## Table and multi-line text arguments
You can include tables and multi-line arguments in scenario steps. These are used by the [[step definitions]] and are either passed as additional `Table` or `string` arguments.

More details can be found [[here|https://cucumber.io/docs/gherkin/reference/#step-arguments]].

## Tags
Tags are markers that can be assigned to features and scenarios. Assigning a tag to a feature is equivalent to assigning the tag to all scenarios in the feature file. More details can be found [[here|https://cucumber.io/docs/cucumber/api/#tags]].

If supported by the [[unit test framework|Unit Test Providers]], SpecFlow generates categories from the tags. The generated category name is the same as the tag's name, but without the leading `@`. You can filter and group the tests to be executed using these unit test categories. For example, you can tag crucial tests with `@important`, and then execute these tests more frequently.

If your unit test framework does not support categories, you can still use tags to implement special logic for tagged scenarios in [[hooks]], [[scoped bindings]] or step definitions by querying the `ScenarioContext.Current.ScenarioInfo.Tags` property.

SpecFlow treats the `@ignore` tag as a special tag. SpecFlow generates an [[ignored unit test|Test-result#ignored-tests]] method from scenarios with this tag.

## Background
The background language element allows specifying a common precondition for all scenarios in a feature file. The background part of the file can contain one or more scenario steps that are executed before any other steps of the scenarios. More details can be found [[here|https://cucumber.io/docs/gherkin/reference/#background]].

SpecFlow generates a method from the background elements that is invoked from all unit tests generated for the scenarios.

## Scenario Outlines
Scenario outlines can be used to define data-driven acceptance tests. They can be also seen as scenario templates. The scenario outline always consists of a scenario template specification (a scenario with data placeholders using the `'<placeholder>'` syntax) and a set of examples that provide values for the placeholders. More details can be found [[here|https://cucumber.io/docs/gherkin/reference/#scenario-outline]]. Note that placing single quotation marks (') around placeholders (e.g. `'<placeholder>'` ) is not mandatory according to the Gherkin language, but helps to improve SpecFlow's ability to parse the scenario outline and generate more accurate regular expressions and test method signatures.

If the [[unit test framework|Unit Test Providers]] supports it, SpecFlow generates row based tests from scenario outlines. Otherwise it generates a parameterized unit test logic method for a scenario outline and an individual unit test method for each example set. 

For better traceability, the generated unit test method names are derived from the scenario outline title and the first value of the examples (first column of the examples table). It is therefore good practice to choose a unique and descriptive parameter as the first column in the example set. As the Gherkin syntax does not require all example columns to have matching placeholders in the scenario outline, you can even introduce an arbitrary column in the example sets used to name tests with more readability. 

SpecFlow performs the placeholder substitution as a separate phase before matching the step bindings. The implementation and the parameters in the step bindings are thus independent of whether they are executed through a direct scenario or a scenario outline. This allows you to later specify further examples in the acceptance tests without changing the step bindings.

**Hint:** In certain cases, when generating method names using the regular expression method, SpecFlow is unable to generate the correct parameter signatures for unit test logic methods without a little help. Placing single quotation marks (`'`) around placeholders (eg. `'<placeholder>'`)improves SpecFlow's ability to parse the scenario outline and generate more accurate regular expressions and test method signatures.

## Comments
You can add comment lines to the feature files at any place by starting the line with `#`. Be careful however, as comments in your specification can be a sign that acceptance criteria have been specified wrongly. 

Comment lines are ignored by SpecFlow.

To comment out blocks, use triple quotes (") to start and to end the block comment.

"""

Given I want to comment this line

Given I want to comment this line

Given I want to comment this line

"""