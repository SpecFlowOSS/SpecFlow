using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class SpecFlowConfigurationSteps
    {
        private readonly ConfigurationDriver _configurationDriver;
        private readonly XmlConfigurationParserDriver _xmlConfigurationParserDriver;
        private readonly JsonConfigurationLoaderDriver _jsonConfigurationLoaderDriver;
        private readonly ConfigurationLoaderDriver _configurationLoaderDriver;

        public SpecFlowConfigurationSteps(
            ConfigurationDriver configurationDriver,
            XmlConfigurationParserDriver xmlConfigurationParserDriver,
            JsonConfigurationLoaderDriver jsonConfigurationLoaderDriver,
            ConfigurationLoaderDriver configurationLoaderDriver)
        {
            _configurationDriver = configurationDriver;
            _xmlConfigurationParserDriver = xmlConfigurationParserDriver;
            _jsonConfigurationLoaderDriver = jsonConfigurationLoaderDriver;
            _configurationLoaderDriver = configurationLoaderDriver;
        }

        [Given(@"there is a project with this specflow\.json configuration")]
        public void GivenThereIsAProjectWithThisSpecFlowJsonConfiguration(string specFlowJson)
        {
            _jsonConfigurationLoaderDriver.AddSpecFlowJson(specFlowJson);
        }

        [Given(@"the specflow configuration is")]
        public void GivenTheSpecFlowConfigurationIs(string specFlowSection)
        {
            var specFlowConfiguration = _xmlConfigurationParserDriver.ParseSpecFlowSection(specFlowSection);
            _configurationLoaderDriver.SetFromSpecFlowConfiguration(specFlowConfiguration);
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
        public void GivenSpecFlowIsConfiguredInTheSpecFlowJson()
        {
            _configurationDriver.SetConfigurationFormat(ConfigurationFormat.Json);
        }
        
        [Given(@"obsoleteBehavior configuration value is set to (.*)")]
        public void GivenObsoleteBehaviorConfigurationValueIsSetTo(string obsoleteBehaviorValue)
        {
//          var configText = $@"<specFlow>
//          <runtime obsoleteBehavior=""{obsoleteBehaviorValue}"" />
//          </specFlow >";

//          GivenTheSpecflowConfigurationIs(configText);
            _configurationDriver.SetRuntimeObsoleteBehavior(obsoleteBehaviorValue);
        }

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
