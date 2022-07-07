# Cucumber Expression support in SpecFlow

Cucumber Expression is an expression type to specify [step definitions](Step-Definitions.md). Cucumber Expressions is an alternative to [Regular Expressions](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions) with a more intuitive syntax.

You can find a detailed description about cucumber expressions [on GitHub](https://github.com/cucumber/cucumber-expressions#readme). In this page we only provide a short summary and the special handling in .NET / SpecFlow.

Cucumber Expressions are supported natively by SpecFlow from v4. In earlier versions you need to add a reference to an additional NuGet package, e.g. for SpecFlow v3.9, you need to use the [CucumberExpressions.SpecFlow.3-9](https://www.nuget.org/packages/CucumberExpressions.SpecFlow.3-9) package.

The following step definition that uses cucumber expression matches to the step `When I have 42 cucumbers in my belly`

```
[When("I have {int} cucumbers in my belly")]
public void WhenIHaveCucumbersInMyBelly(int count) { ... }
```

## Cucumber Expression basics

### Simple text

To match for a simple text, just use the text as cucumber expression. 

`[When("I do something")]` matches to `When I do something`

### Parameters

Parameters can be defined using the `{parameter-type}` syntax. Where `parameter-type` can be any of the following:

* A short name for some simple built-in types: `{int}`, `{long}`, `{byte}`, `{float}`, `{double}`, `{decimal}`
* `string` (`{string}`) that matches to quoted text wrapped with either `"` or `'`. E.g., `[Given("a user {string}")]` matches to `Given a user "Marvin"` or `Given a user 'Zaphod Beeblebrox'`.
* `word` (`{word}`) that matches to a single word without quotes. E.g., `[Given("a user {word}")]` matches to `Given a user Marvin`.
* Empty (`{}`) that matches to anything (like `(.*)` with regex).
* A type name without namespace that is supported by SpecFlow as a parameter type (types with built-in support, enum types and types with [custom argument conversions](Step-Argument-Conversions.md)). E.g. `[When("I have {CustomColor} cucumbers in my belly")]` matches to `When I have green cucumbers in my belly` if `CustomColor` is an enum with `Green` as a value.
* A custom type name you have specified int the `[StepArgumentTransformation]` attribute. E.g., With `[StepArgumentTransformation("v(.*)", Name = "my_version")]`, you can define a step as `[When("I download the release {my_version} of the application")]` that matches to `When I download the release v1.2.3 of the application`.

### Optionals, alternatives

Cucumber expressions use the parentheses (`(...)`) for optionals and the `/` character to define alternatives. The step definition 

```
[When("I have {int} cucumber(s) in my belly/tummy")]
public void WhenIHaveCucumbersInMyBelly(int count)
```

will match to all of the following steps

```
When I have 42 cucumbers in my belly
When I have 1 cucumber in my belly
When I have 8 cucumbers in my tummy
```

## Using Cucumber Expressions with SpecFlow

When you use SpecFlow v4 or have installed the package for the earlier versions, you can use both cucumber expressions and regular expressions in your project. SpecFlow has uses some heuristics to decide if your expression is a cucumber expression or a regular expression.

In case your regular expression is wrongly detected as cucumber expression, you can always force to use regular expression by specifying the regex start/end markers (`^`/`$`).

```
[When(@"^this expression is treated as a regex$")]
```

For potential migration problems of your existing SpecFlow codebase with regular expressions, please check the [Upgrade from SpecFlow 3.* to 4.*](../Guides/UpgradeSpecFlow3To4.md) guide.

### IDE Support for Cucumber Expressions

Recognition of step definitions using cucumber expressions (including "Go To Definition" navigation) is only supported in Visual Studio 2022, or in Visual Studio 2017/20017 using the "Deveroom" Visual Studio extension.

In older versions you will see these steps as "undefined step", but the test execution is not affected by this problem.
