using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi.Driver;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi._5_TestRun;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ConfigurationSteps
    {
        private readonly VSTestExecutionDriver _vstestExecutionDriver;
        private readonly ConfigurationDriver _configurationDriver;
        private readonly TestRunConfiguration _testRunConfiguration;

        public ConfigurationSteps(VSTestExecutionDriver vstestExecutionDriver, ConfigurationDriver configurationDriver, TestRunConfiguration testRunConfiguration)
        {
            _vstestExecutionDriver = vstestExecutionDriver;
            _configurationDriver = configurationDriver;
            _testRunConfiguration = testRunConfiguration;
        }

        [Then(@"the app\.config is used for configuration")]
        public void ThenTheApp_ConfigIsUsedForConfiguration()
        {
            if (_testRunConfiguration.UnitTestProvider == TestProjectGenerator.UnitTestProvider.MSTest)
            {
                _vstestExecutionDriver.LastTestExecutionResult.TestResults.Where(tr => tr.StdOut.Contains("Using app.config")).Should().NotBeEmpty();
            }
            else
            {
                _vstestExecutionDriver.LastTestExecutionResult.Output.Should().Contain("Using app.config");
            }
        }

        [Then(@"the specflow\.json is used for configuration")]
        public void ThenTheSpecflow_JsonIsUsedForConfiguration()
        {
            _vstestExecutionDriver.LastTestExecutionResult.Output.Should().Contain("Using specflow.json");
        }

        [Given(@"the feature language is '(.*)'")]
        public void GivenTheFeatureLanguageIs(string featureLanguage)
        {
            _configurationDriver.SetFeatureLanguage(featureLanguage);
        }
    }
}
