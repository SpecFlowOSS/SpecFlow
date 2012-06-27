using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.IdeIntegration.Bindings
{
    public class IdeBindingSourceProcessor : BindingSourceProcessor
    {
        private List<IStepDefinitionBinding> stepDefinitionBindings = new List<IStepDefinitionBinding>();

        public IdeBindingSourceProcessor() : base(new BindingFactory(new StepDefinitionRegexCalculator(new RuntimeConfiguration())))
        {
        }

        protected override void ProcessStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            stepDefinitionBindings.Add(stepDefinitionBinding);
        }

        public IEnumerable<IStepDefinitionBinding> ReadStepDefinitionBindings()
        {
            var result = stepDefinitionBindings;
            stepDefinitionBindings = new List<IStepDefinitionBinding>();
            return result;
        }
    }
}