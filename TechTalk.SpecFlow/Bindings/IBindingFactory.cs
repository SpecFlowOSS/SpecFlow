using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingFactory
    {
        IHookBinding CreateHookBinding(IBindingMethod bindingMethod, HookType hookType, BindingScope bindingScope,
            int hookOrder);

        IStepDefinitionBindingBuilder CreateStepDefinitionBindingBuilder(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, 
            BindingScope bindingScope, string expressionString);

        IStepArgumentTransformationBinding CreateStepArgumentTransformation(string regexString,
            IBindingMethod bindingMethod, string parameterTypeName = null);
    }
}