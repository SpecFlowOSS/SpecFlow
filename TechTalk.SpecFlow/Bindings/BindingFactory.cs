using TechTalk.SpecFlow.Bindings.CucumberExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public class BindingFactory : IBindingFactory
{
    private readonly IStepDefinitionRegexCalculator stepDefinitionRegexCalculator;
    private readonly ICucumberExpressionStepDefinitionBindingBuilderFactory _cucumberExpressionStepDefinitionBindingBuilderFactory;

    public BindingFactory(IStepDefinitionRegexCalculator stepDefinitionRegexCalculator, ICucumberExpressionStepDefinitionBindingBuilderFactory cucumberExpressionStepDefinitionBindingBuilderFactory)
    {
        this.stepDefinitionRegexCalculator = stepDefinitionRegexCalculator;
        _cucumberExpressionStepDefinitionBindingBuilderFactory = cucumberExpressionStepDefinitionBindingBuilderFactory;
    }

    public IHookBinding CreateHookBinding(IBindingMethod bindingMethod, HookType hookType, BindingScope bindingScope,
        int hookOrder)
    {
        return new HookBinding(bindingMethod, hookType, bindingScope, hookOrder);
    }

    public IStepDefinitionBindingBuilder CreateStepDefinitionBindingBuilder(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string expressionString)
    {
        return expressionString == null
            ? new MethodNameStepDefinitionBindingBuilder(stepDefinitionRegexCalculator, stepDefinitionType, bindingMethod, bindingScope)
            : CucumberExpressionStepDefinitionBindingBuilder.IsCucumberExpression(expressionString)
                ? _cucumberExpressionStepDefinitionBindingBuilderFactory.Create(stepDefinitionType, bindingMethod, bindingScope, expressionString)
                : new RegexStepDefinitionBindingBuilder(stepDefinitionType, bindingMethod, bindingScope, expressionString);
    }

    public IStepArgumentTransformationBinding CreateStepArgumentTransformation(string regexString,
        IBindingMethod bindingMethod, string parameterTypeName = null)
    {
        return new StepArgumentTransformationBinding(regexString, bindingMethod, parameterTypeName);
    }
}
