using FluentAssertions;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi.Driver;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi._5_TestRun;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ConfigurationSteps
    {
        private readonly VSTestExecutionDriver _vstestExecutionDriver;
        private readonly ConfigurationDriver _configurationDriver;

        public ConfigurationSteps(VSTestExecutionDriver vstestExecutionDriver, ConfigurationDriver configurationDriver)
        {
            _vstestExecutionDriver = vstestExecutionDriver;
            _configurationDriver = configurationDriver;
        }

        [Then(@"the app\.config is used for configuration")]
        public void ThenTheApp_ConfigIsUsedForConfiguration()
        {
            _vstestExecutionDriver.LastTestExecutionResult.Output.Should().Contain("Using app.config");
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
