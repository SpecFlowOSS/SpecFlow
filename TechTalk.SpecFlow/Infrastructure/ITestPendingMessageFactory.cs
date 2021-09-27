namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ITestPendingMessageFactory
    {
        string BuildFromScenarioContext(ScenarioContext scenarioContext);
    }
}
