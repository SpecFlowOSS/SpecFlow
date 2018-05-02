using SpecFlow.TestProjectGenerator.NewApi.Driver;
using SpecFlow.TestProjectGenerator.NewApi._1_Memory;
using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class SpecFlowConfigurationSteps
    {
        private readonly AppConfigConfigurationDriver _appConfigConfigurationDriver;
        private readonly ConfigurationDriver _configurationDriver;

        public SpecFlowConfigurationSteps(AppConfigConfigurationDriver appConfigConfigurationDriver, ConfigurationDriver configurationDriver)
        {
            this._appConfigConfigurationDriver = appConfigConfigurationDriver;
            _configurationDriver = configurationDriver;
        }

        [Given(@"the specflow configuration is")]
        public void GivenTheSpecflowConfigurationIs(string specFlowConfigurationContent)
        {
            _configurationDriver.AddFromXmlSpecFlowSection(specFlowConfigurationContent);
        }

        [Given(@"the project is configured to use the (.+) provider")]
        public void GivenTheProjectIsConfiguredToUseTheUnitTestProvider(string providerName)
        {
            _configurationDriver.SetUnitTestProvider(providerName);
        }


        [Given(@"SpecFlow is configured in the app\.config")]
        public void GivenSpecFlowIsConfiguredInTheApp_Config()
        {
            _configurationDriver.SetConfigurationFormat(ConfigurationFormat.Config);
        }

        [Given(@"SpecFlow is configured in the specflow\.json")]
        public void GivenSpecFlowIsConfiguredInTheSpecflow_Json()
        {
            _configurationDriver.SetConfigurationFormat(ConfigurationFormat.Json);
        }


        [StepArgumentTransformation(@"enabled")]
        public bool ConvertEnabled() { return true; }

        [StepArgumentTransformation(@"disabled")]
        public bool ConvertDisabled() { return false; }

        [Given(@"row testing is (.+)")]
        public void GivenRowTestingIsRowTest(bool enabled)
        {
            _appConfigConfigurationDriver.SetRowTest(enabled);
        }

        [Given(@"the type '(.*)' is registered as '(.*)' in SpecFlow runtime configuration")]
        public void GivenTheTypeIsRegisteredAsInSpecFlowRuntimeConfiguration(string typeName, string interfaceName)
        {
            _appConfigConfigurationDriver.AddRuntimeDependencyCustomization(typeName, interfaceName);
        }
    }
}
