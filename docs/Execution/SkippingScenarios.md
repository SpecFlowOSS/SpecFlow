# Skipping Scenarios

Since SpecFlow 3.1 you can do skip programmatically Scenarios with the UnitTestRuntimeProvider.

## Example Code

``` csharp

    [Binding]
    public sealed class StepDefinitions
    {
        private readonly IUnitTestRuntimeProvider _unitTestRuntimeProvider;

        public CalculatorStepDefinitions(IUnitTestRuntimeProvider unitTestRuntimeProvider)
        {
            _unitTestRuntimeProvider = unitTestRuntimeProvider;
        }

        [When("your binding")]
        public void YourBindingMethod()
        {
            _unitTestRuntimeProvider.TestIgnore("This scenario is always skipped");
        }
    }
```

Ignoring is like skipping the scenario. Be careful, as it behaves a little bit different for the different unit test runners (xUnit, NUnit, MSTest, SpecFlow+ Runner).

## Limitations

Currently this works only in [step definitions](../Bindings/Step-Definitions.md). It is not possible to use it in [hooks](../Bindings/Hooks.md). See GitHub Issue [#2059](https://github.com/SpecFlowOSS/SpecFlow/issues/2059)

