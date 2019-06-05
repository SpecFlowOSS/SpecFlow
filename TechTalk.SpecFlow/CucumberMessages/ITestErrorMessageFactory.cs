namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ITestErrorMessageFactory
    {
        string BuildFromScenarioContext(ScenarioContext scenarioContext);
    }
}
