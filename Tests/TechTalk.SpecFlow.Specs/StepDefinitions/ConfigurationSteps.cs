using FluentAssertions;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi._5_TestRun;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ConfigurationSteps
    {
        private readonly VSTestExecutionDriver _vstestExecutionDriver;

        public ConfigurationSteps(VSTestExecutionDriver vstestExecutionDriver)
        {
            _vstestExecutionDriver = vstestExecutionDriver;
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

    }
}
