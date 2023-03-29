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
        private readonly ConfigurationLoaderDriver _configurationLoaderDriver;
        private readonly TestSuiteSetupDriver _testSuiteSetupDriver;

        public SpecFlowConfigurationSteps(
            ConfigurationDriver configurationDriver,
            XmlConfigurationParserDriver xmlConfigurationParserDriver,
            ConfigurationLoaderDriver configurationLoaderDriver,
            TestSuiteSetupDriver testSuiteSetupDriver)
        {
            _configurationDriver = configurationDriver;
            _xmlConfigurationParserDriver = xmlConfigurationParserDriver;
            _configurationLoaderDriver = configurationLoaderDriver;
            _testSuiteSetupDriver = testSuiteSetupDriver;
        }

        [Given(@"the project has no specflow\.json configuration")]
        public void GivenTheProjectHasNoSpecflow_JsonConfiguration()
        {
            _configurationDriver.SetConfigurationFormat(ConfigurationFormat.None);
        }

        [Given(@"the project has no app\.config configuration")]
        public void GivenTheProjectHasNoApp_ConfigConfiguration()
        {
            _configurationDriver.SetConfigurationFormat(ConfigurationFormat.None);
        }

        [Given(@"there is a project with this specflow\.json configuration")]
        public void GivenThereIsAProjectWithThisSpecFlowJsonConfiguration(string specFlowJson)
        {
            _testSuiteSetupDriver.AddSpecFlowJsonFromString(specFlowJson);
        }

        [Given(@"there is a project with this app\.config configuration")]
        public void GivenThereIsAProjectWithThisApp_ConfigConfiguration(string multilineText)
        {
            _testSuiteSetupDriver.AddAppConfigFromString(multilineText);
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
        
        [Given(@"there is a scenario")]
        public void GivenThereIsAScenario()
        {
            _testSuiteSetupDriver.AddScenarios(1);
        }
    }
}
