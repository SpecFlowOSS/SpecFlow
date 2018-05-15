using SpecFlow.TestProjectGenerator.NewApi.Driver;
using SpecFlow.TestProjectGenerator.NewApi._1_Memory;
using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class SpecFlowConfigurationSteps
    {
        private readonly ConfigurationDriver _configurationDriver;
        private readonly XmlConfigurationLoaderDriver _xmlConfigurationLoaderDriver;

        public SpecFlowConfigurationSteps(ConfigurationDriver configurationDriver, XmlConfigurationLoaderDriver xmlConfigurationLoaderDriver)
        {
            _configurationDriver = configurationDriver;
            _xmlConfigurationLoaderDriver = xmlConfigurationLoaderDriver;
        }

        [Given(@"the specflow configuration is")]
        public void GivenTheSpecflowConfigurationIs(string specFlowSection)
        {
            _xmlConfigurationLoaderDriver.AddFromXmlSpecFlowSection(specFlowSection);
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
        
        [Given(@"obsoleteBehavior configuration value is set to (.*)")]
        public void GivenObsoleteBehaviorConfigurationValueIsSetTo(string obsoleteBehaviorValue)
        {
            var configText = $@"<specFlow>
			    <runtime obsoleteBehavior=""{obsoleteBehaviorValue}"" />
                </specFlow >";

            GivenTheSpecflowConfigurationIs(configText);
        }

        [StepArgumentTransformation(@"enabled")]
        public bool ConvertEnabled() { return true; }

        [StepArgumentTransformation(@"disabled")]
        public bool ConvertDisabled() { return false; }

        [Given(@"row testing is (.+)")]
        public void GivenRowTestingIsRowTest(bool enabled)
        {
            _configurationDriver.SetIsRowTestsAllowed(enabled);
        }

        [Given(@"the type '(.*)' is registered as '(.*)' in SpecFlow runtime configuration")]
        public void GivenTheTypeIsRegisteredAsInSpecFlowRuntimeConfiguration(string typeName, string interfaceName)
        {
            _configurationDriver.AddRuntimeRegisterDependency(typeName, interfaceName);
        }
    }
}
