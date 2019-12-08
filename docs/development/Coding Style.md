# Coding Style

1. Rule: use the same coding style as already used!

## Static versus Instance Methods

We prefer instance methods, even if they can be made static because they do not use instance members. Making a static methods into an instance method happens relatively often and can entail a lot of work.

## Naming Conventions for Tests

The test class should be named like the class it is testing, with a `Tests` suffix.
So for example:  if a class is named `Calculator`, then the test class is called `CalculatorTests`.

Each test method is named by three parts, separated by an underscore. The parts are "method or property under test", "scenario" and "expected result". For example, if we want to test the `Add` method with a small positive and a big negative argument and the result should be negative, then the text method would be called `Add_SmallPositiveAndBigNegativeArgument_ResultShouldBeNegative`.

## Private fields

Private fields begin with a `_` (underscore).