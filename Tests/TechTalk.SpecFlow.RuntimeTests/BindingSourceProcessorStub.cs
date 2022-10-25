using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.CucumberExpressions;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class BindingSourceProcessorStub : BindingSourceProcessor, IRuntimeBindingSourceProcessor
    {
        public readonly List<IStepDefinitionBinding> StepDefinitionBindings = new();
        public readonly List<IHookBinding> HookBindings = new();
        public readonly List<IStepArgumentTransformationBinding> StepArgumentTransformationBindings = new();
        public readonly List<string> ValidationErrors = new();

        public BindingSourceProcessorStub() : base(new BindingFactory(new StepDefinitionRegexCalculator(ConfigurationLoader.GetDefault()), new CucumberExpressionStepDefinitionBindingBuilderFactory(new CucumberExpressionParameterTypeRegistry(new BindingRegistry()))))
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

        protected override bool OnValidationError(string messageFormat, params object[] arguments)
        {
            ValidationErrors.Add(string.Format(messageFormat, arguments));
            return base.OnValidationError(messageFormat, arguments);
        }
    }
}