using System;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingFactory
    {
        EventBinding CreateEventBinding(MethodInfo methodInfo, BindingScope bindingScope);
        StepBinding CreateStepBinding(BindingType type, string regexString, MethodInfo methodInfo, BindingScope bindingScope);
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

        public EventBinding CreateEventBinding(MethodInfo methodInfo, BindingScope bindingScope)
        {
            return new EventBinding(runtimeConfiguration, errorProvider, methodInfo, bindingScope);
        }

        public StepBinding CreateStepBinding(BindingType type, string regexString, MethodInfo methodInfo, BindingScope bindingScope)
        {
            return new StepBinding(runtimeConfiguration, errorProvider, type, regexString, methodInfo, bindingScope);
        }

        public StepTransformationBinding CreateStepArgumentTransformation(string regexString, MethodInfo methodInfo)
        {
            return new StepTransformationBinding(runtimeConfiguration, errorProvider, regexString, methodInfo);
        }
    }
}