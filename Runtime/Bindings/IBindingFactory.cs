using System;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
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
        private readonly RuntimeConfiguration runtimeConfiguration;
        private readonly IErrorProvider errorProvider;
        private readonly IStepDefinitionRegexCalculator stepDefinitionRegexCalculator;

        internal BindingFactory(RuntimeConfiguration runtimeConfiguration, IErrorProvider errorProvider, IStepDefinitionRegexCalculator stepDefinitionRegexCalculator)
        {
            this.runtimeConfiguration = runtimeConfiguration;
            this.errorProvider = errorProvider;
            this.stepDefinitionRegexCalculator = stepDefinitionRegexCalculator;
        }

        public IHookBinding CreateEventBinding(IBindingMethod bindingMethod, BindingScope bindingScope)
        {
            return new HookBinding(runtimeConfiguration, errorProvider, bindingMethod, bindingScope);
        }

        public StepDefinitionBinding CreateStepBinding(StepDefinitionType type, string regexString, IBindingMethod bindingMethod, BindingScope bindingScope)
        {
            if (regexString == null)
                regexString = stepDefinitionRegexCalculator.CalculateRegexFromMethod(type, bindingMethod);
            return new StepDefinitionBinding(runtimeConfiguration, errorProvider, type, regexString, bindingMethod, bindingScope);
        }

        public StepTransformationBinding CreateStepArgumentTransformation(string regexString, IBindingMethod bindingMethod)
        {
            return new StepTransformationBinding(runtimeConfiguration, errorProvider, regexString, bindingMethod);
        }
    }
}