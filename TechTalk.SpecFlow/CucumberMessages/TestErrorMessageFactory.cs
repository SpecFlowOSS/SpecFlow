using System;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class TestErrorMessageFactory : ITestErrorMessageFactory
    {
        public string BuildFromScenarioContext(ScenarioContext scenarioContext)
        {
            return scenarioContext.TestError?.ToString() ?? "Test failed with an unknown error";
        }
    }
}
