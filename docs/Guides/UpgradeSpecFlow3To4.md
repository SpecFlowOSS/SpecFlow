# Upgrade from SpecFlow 3.* to 4.*

This guide explains how to update your SpecFlow 3.* project to the latest SpecFlow 4.* version

## Make a Backup!

Before upgrading to the latest version, ensure you have a backup of your project (either locally or in a source control system).

## Cucumber Expressions support, compatibility of existing expressions

SpecFlow v4 supports [Cucumber Expressions](../Bindings/Cucumber-Expressions.md) natively for [step definitions](../Bindings/Step-Definitions.md). This means that whenever you define a step using the `[Given]`, `[When]` or `[Then]` attribute you can either provide a regular expression for it as a parameter or a cucumber expression.

Based on the expression you have provided, SpecFlow tries to decide whether it is a regular expression or a cucumber expression.

Most of your existing regex step definitions will be compatible, because they are either properly recognized as regex or the expression works the same way with both expression types (e.g. simple text without parameters). 

### Invalid expressions after upgrade

In some cases you may see an error after upgrading to the new SpecFlow version. For example if you had a step definition with an attribute like:

```
[When(@"I \$ something")]
```

```
This Cucumber Expression has a problem ...
```

In this case the problem is that SpecFlow wrongly identified your expression as a cucumber expression.

**Solution 1:** Force the expression to be a regular expression by specifying the regex start/end markers (`^`/`$`):

```
[When(@"^I \$ something$")]
```

**Solution 2:** Change the expression to be a valid cucumber expression. For the example above, you need to remove the masking character (`\`), because the `$` sign does not have to be masked in cucumber expressions:

```
[When("I $ something")]
```

### Expression matching problems during test execution

In some very special cases it can happen that the expression is wrongly identified as cucumber expression but you only get the step binding error during test execution (usually "No matching step definition found" error), because the expression is valid as regex and as cucumber expression as well, but with different meaning. 

For example if you had a step definition that matches the step `When I a/b something`, that will be considered as a cucumber expression, but in cucumber expressions, the `/` is used for alternation (so it matches to either `When I a something` or `When I b something`).

```
[When(@"I a/b something")]
```

**Solutions:** You can apply the same solutions as above: either force it to be a regular expression by specifying the regex start/end markers or make it a valid cucumber expression. 

For the latter case, you would need to mask the `/` character:

```
[When(@"I a\/b something")]
```

### Cucumber Expression step definition skeletons

From v4 SpecFlow will by default generate step definition skeletons (snippets) for the new steps. So in case you write a new step as

```
When I have 42 cucumbers in my belly
```

SpecFlow will suggest the step definition to be:

```
[When("I have {int} cucumbers in my belly")]
public void WhenIHaveCucumbersInMyBelly(int p0)
...
```

If you would like to use only regular expressions in your project you either have to manually fix the expression, or you can configure SpecFlow to generate skeletons with regular expressions. This you can achieve with the following setting in the `specflow.json` file:

```
{
  "$schema": "https://specflow.org/specflow-config.json",
  "trace": {
    "stepDefinitionSkeletonStyle": "RegexAttribute"
  }
}
```

### IDE Support for Cucumber Expressions

Recognition of step definitions using cucumber expressions (including "Go To Definition" navigation) is only supported in Visual Studio 2022, or in Visual Studio 2017/20017 using the "Deveroom" Visual Studio extension.

In older versions you will see these steps as "undefined step", but the test execution is not affected by this problem.
