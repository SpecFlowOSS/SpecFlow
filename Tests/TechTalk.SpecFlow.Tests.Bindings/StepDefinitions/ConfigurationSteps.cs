using FluentAssertions;
using TechTalk.SpecFlow.Tests.Bindings.Drivers;

namespace TechTalk.SpecFlow.Tests.Bindings.StepDefinitions
{
    [Binding]
    class ConfigurationSteps
    {
        private readonly TestExecutionResult _testExecutionResult;

        public ConfigurationSteps(TestExecutionResult testExecutionResult)
        {
            _testExecutionResult = testExecutionResult;
        }

        [Then(@"the app\.config is used for configuration")]
        public void ThenTheApp_ConfigIsUsedForConfiguration()
        {
            _testExecutionResult.ExecutionLog.Should().Contain("Using app.config");
        }

        [Then(@"the specflow\.json is used for configuration")]
        public void ThenTheSpecflow_JsonIsUsedForConfiguration()
        {
            _testExecutionResult.ExecutionLog.Should().Contain("Using specflow.json");
        }

    }
}
