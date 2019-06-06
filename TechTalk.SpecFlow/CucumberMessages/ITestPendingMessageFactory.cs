namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ITestPendingMessageFactory
    {
        string BuildFromScenarioContext(ScenarioContext scenarioContext);
    }
}
