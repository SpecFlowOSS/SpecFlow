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
        IStepDefinitionBinding CreateStepBinding(StepDefinitionType type, string regexString, IBindingMethod bindingMethod, BindingScope bindingScope);
        IStepArgumentTransformationBinding CreateStepArgumentTransformation(string regexString, IBindingMethod bindingMethod);
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

        public IStepDefinitionBinding CreateStepBinding(StepDefinitionType type, string regexString, IBindingMethod bindingMethod, BindingScope bindingScope)
        {
            if (regexString == null)
                regexString = stepDefinitionRegexCalculator.CalculateRegexFromMethod(type, bindingMethod);
            return new StepDefinitionBinding(type, regexString, bindingMethod, bindingScope);
        }

        public IStepArgumentTransformationBinding CreateStepArgumentTransformation(string regexString, IBindingMethod bindingMethod)
        {
            return new StepArgumentTransformationBinding(regexString, bindingMethod);
        }
    }
}