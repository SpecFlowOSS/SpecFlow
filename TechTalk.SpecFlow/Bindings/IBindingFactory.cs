using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingFactory
    {
        IHookBinding CreateHookBinding(IBindingMethod bindingMethod, HookType hookType, BindingScope bindingScope,
            int hookOrder);

        IStepDefinitionBindingBuilder CreateStepDefinitionBindingBuilder(StepDefinitionType type, IBindingMethod bindingMethod, 
            BindingScope bindingScope, string expressionString);

        IStepDefinitionBinding CreateStepBinding(StepDefinitionType type, string regexString,
            IBindingMethod bindingMethod, BindingScope bindingScope);

        IStepArgumentTransformationBinding CreateStepArgumentTransformation(string regexString,
            IBindingMethod bindingMethod);
    }
}