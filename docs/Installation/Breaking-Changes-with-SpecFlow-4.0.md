# Breaking changes with SpecFlow 4.0

## Cucumber Expressions support, compatibility of existing expressions

SpecFlow v4 supports [Cucumber Expressions](../Bindings/Cucumber-Expressions.md) natively for [step definitions](../Bindings/Step-Definitions.md). This means that whenever you define a step using the `[Given]`, `[When]` or `[Then]` attribute you can either provide a regular expression for it as a parameter or a cucumber expression.

Most of your existing regex step definitions will be compatible, because they are either properly recognized as regex or the expression works the same way with both expression types (e.g. simple text without parameters). 

In case your regular expression is wrongly detected as cucumber expression, you can always force to use regular expression by specifying the regex start/end markers (`^`/`$`).

```
[When(@"^this expression is treated as a regex$")]
```

For potential migration problems of your existing SpecFlow codebase with regular expressions, please check the [Upgrade from SpecFlow 3.* to 4.*](../Guides/UpgradeSpecFlow3To4.md) guide.
