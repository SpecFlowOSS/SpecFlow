using System;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public class MethodNameStepDefinitionBindingBuilder : RegexStepDefinitionBindingBuilder
{
    private readonly IStepDefinitionRegexCalculator _stepDefinitionRegexCalculator;

    public MethodNameStepDefinitionBindingBuilder(IStepDefinitionRegexCalculator stepDefinitionRegexCalculator, StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope)
        : base(stepDefinitionType, bindingMethod, bindingScope, bindingMethod.Name)
    {
        _stepDefinitionRegexCalculator = stepDefinitionRegexCalculator;
    }

    protected override string GetRegexSource(out string expressionType)
    {
        expressionType = StepDefinitionExpressionTypes.MethodName;
        return _stepDefinitionRegexCalculator.CalculateRegexFromMethod(_stepDefinitionType, _bindingMethod);
    }
}