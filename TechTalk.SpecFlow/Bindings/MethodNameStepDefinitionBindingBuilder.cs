using System;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public class MethodNameStepDefinitionBindingBuilder : RegexStepDefinitionBindingBuilder
{
    private readonly IStepDefinitionRegexCalculator _stepDefinitionRegexCalculator;
    private readonly StepDefinitionType _stepDefinitionType;
    private readonly IBindingMethod _bindingMethod;
    private readonly BindingScope _bindingScope;

    public MethodNameStepDefinitionBindingBuilder(IStepDefinitionRegexCalculator stepDefinitionRegexCalculator, StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope)
        : base(stepDefinitionType, bindingMethod, bindingScope, bindingMethod.Name)
    {
        _stepDefinitionRegexCalculator = stepDefinitionRegexCalculator;
        _stepDefinitionType = stepDefinitionType;
        _bindingMethod = bindingMethod;
        _bindingScope = bindingScope;
    }

    protected override string GetRegexSource()
    {
        return _stepDefinitionRegexCalculator.CalculateRegexFromMethod(_stepDefinitionType, _bindingMethod);
    }
}