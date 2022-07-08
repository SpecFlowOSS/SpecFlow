using System;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings.CucumberExpressions;

public interface ICucumberExpressionStepDefinitionBindingBuilderFactory
{
    CucumberExpressionStepDefinitionBindingBuilder Create(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string sourceExpression);
}

public class CucumberExpressionStepDefinitionBindingBuilderFactory
    : ICucumberExpressionStepDefinitionBindingBuilderFactory
{
    private readonly CucumberExpressionParameterTypeRegistry _cucumberExpressionParameterTypeRegistry;

    public CucumberExpressionStepDefinitionBindingBuilderFactory(CucumberExpressionParameterTypeRegistry cucumberExpressionParameterTypeRegistry)
    {
        _cucumberExpressionParameterTypeRegistry = cucumberExpressionParameterTypeRegistry;
    }

    public CucumberExpressionStepDefinitionBindingBuilder Create(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string sourceExpression)
    {
        _cucumberExpressionParameterTypeRegistry.OnBindingMethodProcessed(bindingMethod);
        return new CucumberExpressionStepDefinitionBindingBuilder(_cucumberExpressionParameterTypeRegistry, stepDefinitionType, bindingMethod, bindingScope, sourceExpression);
    }
}