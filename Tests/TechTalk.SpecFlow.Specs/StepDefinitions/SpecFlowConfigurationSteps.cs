using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class SpecFlowConfigurationSteps
    {
        private readonly SpecFlowConfigurationDriver specFlowConfigurationDriver;

        public SpecFlowConfigurationSteps(SpecFlowConfigurationDriver specFlowConfigurationDriver)
        {
            this.specFlowConfigurationDriver = specFlowConfigurationDriver;
        }

        [Given(@"the specflow configuration is")]
        public void GivenTheSpecflowConfigurationIs(string specFlowConfigurationContent)
        {
            specFlowConfigurationDriver.SetSpecFlowConfigurationContent(specFlowConfigurationContent);
        }

        [Given(@"the project is configured to use the (.+) provider")]
        public void GivenTheProjectIsConfiguredToUseTheUnitTestProvider(string providerName)
        {
            specFlowConfigurationDriver.SetUnitTestProvider(providerName);
        }

        [StepArgumentTransformation(@"enabled")]
        public bool ConvertEnabled() { return true; }

        [StepArgumentTransformation(@"disabled")]
        public bool ConvertDisabled() { return false; }

        [Given(@"row testing is (.+)")]
        public void GivenRowTestingIsRowTest(bool enabled)
        {
            specFlowConfigurationDriver.SetRowTest(enabled);
        }
    }
}
