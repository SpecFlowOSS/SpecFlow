# Step Definition Styles

[[Step definitions]] provide the connection between the free-text specification steps and the application interfaces. Step definitions are .NET methods that match to certain scenario steps.

The classic way of providing the match rules is to annotate the method with regular expressions. From SpecFlow 1.9 however, you can also create the step definitions without regex.

This page contains information about the **different styles**, the **step definition skeleton generation** and about the **custom templates**. For general rules of step definitions, please check the [[Step Definitions]] page.

## Step Definition Styles

### Regular expressions in attributes

This is the classic way of specifying the step definitions. The step definition method has to be annotated with one or more step definition attributes with regular expressions.

```c#
[Given(@"I have entered (.*) into the calculator")]
public void GivenIHaveEnteredNumberIntoTheCalculator(int number)
{
  ...
}
```

Regular expression matching rules:

* Regular expressions are always matched to the entire step, even if you do not use the `^` and `$` markers.
* The capturing groups (`(…)`) in the regular expression define the arguments for the method in order (the result of the first group becomes the first argument etc.).
* You can use non-capturing groups `(?:regex)` in order to use groups without a method argument.

### Method name - underscores

Most of the step definitions can also be specified without regular expression: the method should be named with underscored naming style (or pascal-case, see below) and should be annotated with an empty `[Given]`, `[When]`, `[Then]` or `[StepDefinition]` attribute. You can refer to the method parameters with either the parameter name (ALL-CAPS) or the parameter index (zero-based) with the `P` prefix, e.g. `P0`.

```c#
[Given]
public void Given_I_have_entered_NUMBER_into_the_calculator(int number)
{
  ...
}
```

Matching rules:

* The match is case-insensitive.
* Underscore character is matched to one or more non-word character (eg. whitespace, punctuation): `\W+`.
* If the step contains accented characters, the method name should also contain the accented characters (no substitution). 
* The step keyword (e.g. `Given`) can be omitted: `[Given] public void I_have_entered_NUMBER_...`.
* The step keyword can be specified in the local Gherkin language, or English. The default language can be specified in the [[app config|Configuration]] as the feature language or binding culture. The following step definition is threfore a valid "Given" step with German language settings: `[When] public void Wenn_ich_addieren_drücke()`

More detailed examples can be found in our specs: [[StepDefinitionsWithoutRegex.feature|https://github.com/techtalk/SpecFlow/blob/master/Tests/TechTalk.SpecFlow.Specs/Features/Execution/StepDefinitionsWithoutRegex.feature]].

### Method name - Pascal-case

Similarly to the underscored naming style, you can also specify the step definitions with Pascal-case method names. 

```c#
[Given]
public void GivenIHaveEnteredNUMBERIntoTheCalculator(int number)
{
  ...
}
```

Matching rules:
* All rules of "Method name - underscores" style applied.
* Any potential word boundary (e.g. number-letter, uppercase-lowercase, uppercase-uppercase) is matching to zero or more non-word character (eg. whitespace, punctuation): `\W*`.
* You can mix this style with underscores. For example, the parameter placeholders can be highlighted this way: `GivenIHaveEntered_P0_IntoTheCalculator(int number)`

### Method name - regex (F# only)

F# allows providing any characters in method names, so you can make the regular expression as the method name, if you use [[F# bindings||FSharp Support]].

```F#
let [<Given>] ``I have entered (.*) into the calculator``(number:int) = 
    Calculator.Push(number)
```