using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.IdeIntegration.Bindings
{
    public class IdeBindingSourceProcessor : BindingSourceProcessor
    {
        private readonly IIdeTracer tracer;
        private List<IStepDefinitionBinding> stepDefinitionBindings = new List<IStepDefinitionBinding>();

        public IdeBindingSourceProcessor(IIdeTracer tracer) : base(new BindingFactory(new StepDefinitionRegexCalculator(new RuntimeConfiguration())))
        {
            this.tracer = tracer;
        }

        protected override void ProcessHooks(BindingSourceMethod bindingSourceMethod, BindingScope[] methodScopes)
        {
            //nop - not needed for IDE integration
        }

        protected override void ProcessStepArgumentTransformations(BindingSourceMethod bindingSourceMethod, BindingScope[] methodScopes)
        {
            //nop - not needed for IDE integration
        }

        protected override void ProcessStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            stepDefinitionBindings.Add(stepDefinitionBinding);
        }

        protected override void ProcessHookBinding(IHookBinding hookBinding)
        {
            //nop - not needed for IDE integration
        }

        protected override void ProcessStepArgumentTransformationBinding(IStepArgumentTransformationBinding stepArgumentTransformationBinding)
        {
            //nop - not needed for IDE integration
        }

        public IEnumerable<IStepDefinitionBinding> ReadStepDefinitionBindings()
        {
            var result = stepDefinitionBindings;
            stepDefinitionBindings = new List<IStepDefinitionBinding>();
            return result;
        }

        protected override bool OnValidationError(string messageFormat, params object[] arguments)
        {
            tracer.Trace(messageFormat, "Warning", arguments);
            return base.OnValidationError(messageFormat, arguments);
        }
    }
}