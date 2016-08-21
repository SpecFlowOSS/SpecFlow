using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class BindingSourceProcessorStub : BindingSourceProcessor, IRuntimeBindingSourceProcessor
    {
        public readonly List<IStepDefinitionBinding> StepDefinitionBindings = new List<IStepDefinitionBinding>();
        public readonly List<IHookBinding> HookBindings = new List<IHookBinding>();
        public readonly List<IStepArgumentTransformationBinding> StepArgumentTransformationBindings = new List<IStepArgumentTransformationBinding>();

        public BindingSourceProcessorStub() : base(new BindingFactory(new StepDefinitionRegexCalculator(ConfigurationLoader.GetDefault())))
        {
        }

        protected override void ProcessStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            StepDefinitionBindings.Add(stepDefinitionBinding);
        }

        protected override void ProcessHookBinding(IHookBinding hookBinding)
        {
            HookBindings.Add(hookBinding);
        }

        protected override void ProcessStepArgumentTransformationBinding(IStepArgumentTransformationBinding stepArgumentTransformationBinding)
        {
            StepArgumentTransformationBindings.Add(stepArgumentTransformationBinding);
        }

        public void BuildingCompleted()
        {
            //nop
        }
    }
}