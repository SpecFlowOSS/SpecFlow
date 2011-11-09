using System;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingFactory
    {
        IHookBinding CreateEventBinding(MethodInfo methodInfo, BindingScope bindingScope);
        StepDefinitionBinding CreateStepBinding(BindingType type, string regexString, MethodInfo methodInfo, BindingScope bindingScope);
        StepTransformationBinding CreateStepArgumentTransformation(string regexString, MethodInfo methodInfo);
    }

    class BindingFactory : IBindingFactory
    {
        private readonly RuntimeConfiguration runtimeConfiguration;
        private readonly IErrorProvider errorProvider;

        public BindingFactory(IObjectContainer container)
        {
            this.runtimeConfiguration = container.Resolve<RuntimeConfiguration>();
            this.errorProvider = container.Resolve<IErrorProvider>();
        }

        internal BindingFactory(RuntimeConfiguration runtimeConfiguration, IErrorProvider errorProvider)
        {
            this.runtimeConfiguration = runtimeConfiguration;
            this.errorProvider = errorProvider;
        }

        public IHookBinding CreateEventBinding(MethodInfo methodInfo, BindingScope bindingScope)
        {
            return new HookBinding(runtimeConfiguration, errorProvider, methodInfo, bindingScope);
        }

        public StepDefinitionBinding CreateStepBinding(BindingType type, string regexString, MethodInfo methodInfo, BindingScope bindingScope)
        {
            return new StepDefinitionBinding(runtimeConfiguration, errorProvider, type, regexString, methodInfo, bindingScope);
        }

        public StepTransformationBinding CreateStepArgumentTransformation(string regexString, MethodInfo methodInfo)
        {
            return new StepTransformationBinding(runtimeConfiguration, errorProvider, regexString, methodInfo);
        }
    }
}