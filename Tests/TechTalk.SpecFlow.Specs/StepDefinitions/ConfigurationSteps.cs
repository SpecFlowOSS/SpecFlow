using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ConfigurationSteps
    {
        private readonly VSTestExecutionDriver _vstestExecutionDriver;
        private readonly ConfigurationDriver _configurationDriver;
        private readonly TestRunConfiguration _testRunConfiguration;
        private readonly SolutionDriver _solutionDriver;

        public ConfigurationSteps(VSTestExecutionDriver vstestExecutionDriver, ConfigurationDriver configurationDriver, TestRunConfiguration testRunConfiguration, SolutionDriver solutionDriver)
        {
            _vstestExecutionDriver = vstestExecutionDriver;
            _configurationDriver = configurationDriver;
            _testRunConfiguration = testRunConfiguration;
            _solutionDriver = solutionDriver;
        }


        [Then(@"the app\.config is used for configuration")]
        public void ThenTheApp_ConfigIsUsedForConfiguration()
        {
            _solutionDriver.CheckCompileOutputForString("Using app.config").Should().BeTrue();
        }

        [Then(@"the specflow\.json is used for configuration")]
        public void ThenTheSpecflow_JsonIsUsedForConfiguration()
        {
            _solutionDriver.CheckCompileOutputForString("Using specflow.json").Should().BeTrue();
        }

        [Given(@"the feature language is '(.*)'")]
        public void GivenTheFeatureLanguageIs(string featureLanguage)
        {
            _configurationDriver.SetFeatureLanguage(featureLanguage);
        }
    }
}
