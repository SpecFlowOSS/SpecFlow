# Executing specific Scenarios in your Build pipeline

SpecFlow converts the tags in your feature files to test case categories:

- NUnit: Category
- MSTest: TestCategory
- xUnit: Trait (similar functionality, SpecFlow will insert a Trait attribute with `Category` name)

This category can be used to filter the test execution in your build pipeline. Note that the incorrect filter can lead to no test getting executed.

You don't have to include the `@` prefix in the filter expression.

Learn more about the filters in the [Official documentation](https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests?pivots=xunit).

## Examples

All the examples here are with using `Category`, but if you are using `MsTest` then you should use `TestCategory` instead.

### How to use the filters

[//]: # "example feature file with tag on scenario -> single tag"

There are 2 scenarios where one of them has a tag: `@done`, other one does not have a tag.

```gherkin
Feature: Breakfast

@done
Scenario: Eating cucumbers
  Given there are 12 cucumbers
  When I eat 5 cucumbers
  Then I should have 7 cucumbers

Scenario: Use all the sugar
  Given there is some sugar in the cup
  When I put all the sugar to my coffee
  Then the cup is empty
```

If we would like to run only the scenario with `@done` tag, then the filter should look like:

```bash
Category=done
```

---

[//]: # "example feature file with 2 tag on scenario -> 'one of two tags at a scenario' - or relationship"

There are 2 scenarios where one of them has a tag: `@done`, and the other one has `@automated`.

```gherkin
Feature: Breakfast

@done
Scenario: Eating cucumbers
  Given there are 12 cucumbers
  When I eat 5 cucumbers
  Then I should have 7 cucumbers

@automated
Scenario: Use all the sugar
  Given there is some sugar in the cup
  When I put all the sugar to my coffee
  Then the cup is empty
```

If we would like to run scenarios which has either `@done` or `@automated`:

```bash
Category=done | Category=automated
```

---

[//]: # "example feature file with tag on feature and tag on 1 scenario, other has no tags -> and relationship"

There are 2 scenarios where one of them has a tag: `@done`, and the other one has `@automated`. On Feature level, there is the `@US123` tag.

```gherkin
@US123
Feature: Breakfast

@done
Scenario: Eating cucumbers
  Given there are 12 cucumbers
  When I eat 5 cucumbers
  Then I should have 7 cucumbers

@automated
Scenario: Use all the sugar
  Given there is some sugar in the cup
  When I put all the sugar to my coffee
  Then the cup is empty
```

If we would like to run only those scenarios, which has both `@US123` and `@done`:

```bash
Category=US123 & Category=done
```

[//]: # "example feature file with 2 tags on scenario -> and relationship"

There are 2 scenarios where one of them has two tags: `@done` and `@important`. There is another scenario, which has the `@automated` tag. On Feature level, there is the `@US123` tag.

```gherkin
@US123
Feature: Breakfast

@done @important
Scenario: Eating cucumbers
  Given there are 12 cucumbers
  When I eat 5 cucumbers
  Then I should have 7 cucumbers

@automated
Scenario: Use all the sugar
  Given there is some sugar in the cup
  When I put all the sugar to my coffee
  Then the cup is empty
```

If we would like to run only those scenarios, which has both `@done` and `@important`:

```bash
Category=done & Category=important
```

---

### dotnet test

Use the `--filter` command-line option:

```bash
dotnet test --filter Category=done
```

```bash
dotnet test --filter "Category=us123 & Category=done"
```

```bash
dotnet test --filter "Category=done | Category=automated"
```

### vstest.console.exe

Use the `/TestCaseFilter` command-line option:

```bash
vstest.console.exe "C:\Temp\BookShop.AcceptanceTests.dll" /TestCaseFilter:"Category=done"
```

```bash
vstest.console.exe "C:\Temp\BookShop.AcceptanceTests.dll" /TestCaseFilter:"Category=us123 & Category=done"
```

```bash
vstest.console.exe "C:\Temp\BookShop.AcceptanceTests.dll" /TestCaseFilter:"Category=done | Category=automated"
```

### Azure DevOps - Visual Studio Test task

The filter expression should be provided in the "Test filter criteria" setting in the `Visual Studio Test` task.

![Visual Studio Test task](/_static/images/vstest-task-filter1.png)

![Visual Studio Test task](/_static/images/vstest-task-filter2.png)
