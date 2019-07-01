namespace TechTalk.SpecFlow.CucumberMessages
{
    public class TestAmbiguousMessageFactory : ITestAmbiguousMessageFactory
    {
        public string BuildFromScenarioContext(ScenarioContext scenarioContext)
        {
            return scenarioContext.TestError?.ToString() ?? "Duplicate step binding found";
        }
    }
}
