﻿using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingFactory
    {
        IHookBinding CreateHookBinding(IBindingMethod bindingMethod, HookType hookType, BindingScope bindingScope,
            int hookOrder);

        IStepDefinitionBinding CreateStepBinding(StepDefinitionType type, string regexString,
            IBindingMethod bindingMethod, BindingScope bindingScope);

        IStepArgumentTransformationBinding CreateStepArgumentTransformation(string regexString,
            IBindingMethod bindingMethod);
    }

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

        public IStepDefinitionBinding CreateStepBinding(StepDefinitionType type, string regexString,
            IBindingMethod bindingMethod, BindingScope bindingScope)
        {
            if (regexString == null)
                regexString = stepDefinitionRegexCalculator.CalculateRegexFromMethod(type, bindingMethod);
            return new StepDefinitionBinding(type, regexString, bindingMethod, bindingScope);
        }

        public IStepArgumentTransformationBinding CreateStepArgumentTransformation(string regexString,
            IBindingMethod bindingMethod)
        {
            return new StepArgumentTransformationBinding(regexString, bindingMethod);
        }
    }
}