using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings.Discovery
{
    public interface IRuntimeBindingSourceProcessor : IBindingSourceProcessor
    {
        
    }

    public class RuntimeBindingSourceProcessor : BindingSourceProcessor, IRuntimeBindingSourceProcessor
    {
        private readonly IBindingRegistry bindingRegistry;
        private readonly IErrorProvider errorProvider;
        private readonly ITestTracer testTracer;

        public RuntimeBindingSourceProcessor(IBindingFactory bindingFactory, IBindingRegistry bindingRegistry, IErrorProvider errorProvider, ITestTracer testTracer) : base(bindingFactory)
        {
            this.bindingRegistry = bindingRegistry;
            this.errorProvider = errorProvider;
            this.testTracer = testTracer;
        }

        protected override void ProcessStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            bindingRegistry.RegisterStepDefinitionBinding(stepDefinitionBinding);
        }

        protected override void ProcessHookBinding(IHookBinding hookBinding)
        {
            bindingRegistry.RegisterHookBinding(hookBinding);
        }

        protected override void ProcessStepArgumentTransformationBinding(IStepArgumentTransformationBinding stepArgumentTransformationBinding)
        {
            bindingRegistry.RegisterStepArgumentTransformationBinding(stepArgumentTransformationBinding);
        }

        protected override bool OnValidationError(string messageFormat, params object[] arguments)
        {
            testTracer.TraceWarning("Invalid binding: " + string.Format(messageFormat, arguments));
            return false; //TODO: currently this is a warning only (hence return false), in v2 this will be changed
        }

        protected override bool ValidateHook(BindingSourceMethod bindingSourceMethod, BindingSourceAttribute hookAttribute, HookType hookType)
        {
            //TODO: this call will be refactored when binding error detecttion will be improved in v2 - currently implemented here for backwards compatibility
            if (!IsScenarioSpecificHook(hookType) &&
                !bindingSourceMethod.IsStatic)
                throw errorProvider.GetNonStaticEventError(bindingSourceMethod.BindingMethod);

            return base.ValidateHook(bindingSourceMethod, hookAttribute, hookType);
        }
    }
}
