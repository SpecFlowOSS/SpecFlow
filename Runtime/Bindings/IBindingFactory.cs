using System;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.Bindings.Reflection;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingFactory
    {
        IHookBinding CreateEventBinding(IBindingMethod bindingMethod, BindingScope bindingScope);
        StepDefinitionBinding CreateStepBinding(StepDefinitionType type, string regexString, IBindingMethod bindingMethod, BindingScope bindingScope);
        StepTransformationBinding CreateStepArgumentTransformation(string regexString, IBindingMethod bindingMethod);
    }

    public class BindingFactory : IBindingFactory
    {
        private readonly IStepDefinitionRegexCalculator stepDefinitionRegexCalculator;

        internal BindingFactory(IStepDefinitionRegexCalculator stepDefinitionRegexCalculator)
        {
            this.stepDefinitionRegexCalculator = stepDefinitionRegexCalculator;
        }

        public IHookBinding CreateEventBinding(IBindingMethod bindingMethod, BindingScope bindingScope)
        {
            return new HookBinding(bindingMethod, bindingScope);
        }

        public StepDefinitionBinding CreateStepBinding(StepDefinitionType type, string regexString, IBindingMethod bindingMethod, BindingScope bindingScope)
        {
            if (regexString == null)
                regexString = stepDefinitionRegexCalculator.CalculateRegexFromMethod(type, bindingMethod);
            return new StepDefinitionBinding(type, regexString, bindingMethod, bindingScope);
        }

        public StepTransformationBinding CreateStepArgumentTransformation(string regexString, IBindingMethod bindingMethod)
        {
            return new StepTransformationBinding(regexString, bindingMethod);
        }
    }
}