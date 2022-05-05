using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public class BindingFactory : IBindingFactory
{
    private readonly IStepDefinitionRegexCalculator stepDefinitionRegexCalculator;

    public BindingFactory(IStepDefinitionRegexCalculator stepDefinitionRegexCalculator)
    {
        this.stepDefinitionRegexCalculator = stepDefinitionRegexCalculator;
    }

    public IHookBinding CreateHookBinding(IBindingMethod bindingMethod, HookType hookType, BindingScope bindingScope,
        int hookOrder)
    {
        return new HookBinding(bindingMethod, hookType, bindingScope, hookOrder);
    }

    public IStepDefinitionBindingBuilder CreateStepDefinitionBindingBuilder(StepDefinitionType type, IBindingMethod bindingMethod, BindingScope bindingScope, string expressionString)
    {
        return expressionString == null
            ? new MethodNameStepDefinitionBindingBuilder(stepDefinitionRegexCalculator, type, bindingMethod, bindingScope)
            : new RegexStepDefinitionBindingBuilder(type, bindingMethod, bindingScope, expressionString);
    }

    public IStepArgumentTransformationBinding CreateStepArgumentTransformation(string regexString,
        IBindingMethod bindingMethod)
    {
        return new StepArgumentTransformationBinding(regexString, bindingMethod);
    }
}
