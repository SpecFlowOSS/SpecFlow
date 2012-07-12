using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.IdeIntegration.Bindings
{
    public class IdeBindingSourceProcessor : BindingSourceProcessor
    {
        private List<IStepDefinitionBinding> stepDefinitionBindings = new List<IStepDefinitionBinding>();

        public IdeBindingSourceProcessor() : base(new BindingFactory(new StepDefinitionRegexCalculator(new RuntimeConfiguration())))
        {
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
    }
}