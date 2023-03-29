# Mark Steps as not implemented

To mark a step as not implemented at runtime, you need to throw a `PendingStepException`. The Runtime of SpecFlow will detect this and will report the appropriate test result back to your test runner.

There are multiple ways to throw the exception.

## Using ScenarioContext.Pending helper method

The `ScenarioContext` class has a static helper method to throw the default `PendingStepException`.

Usage sample:

``` csharp
[When("I set the current ScenarioContext to pending")]
public void WhenIHaveAPendingStep()
{
    ScenarioContext.Pending();
}
```

This is the preferred way.

## Throwing the `PendingStepException` by your own

You can also throw the Exception manually. In this case you have the possibility to provide a custom message.

### Default Message

``` csharp
[When("I set the current ScenarioContext to pending")]
public void WhenIHaveAPendingStep()
{
    throw new PendingStepException();
}
```

### Custom Message

``` csharp
[When("I set the current ScenarioContext to pending")]
public void WhenIHaveAPendingStep()
{
    throw new PendingStepException("custom pendingstep message");
}
```