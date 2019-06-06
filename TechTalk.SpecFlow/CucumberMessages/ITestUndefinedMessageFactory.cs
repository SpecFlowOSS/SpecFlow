namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ITestUndefinedMessageFactory
    {
        string BuildFromContext(ScenarioContext scenarioContext, FeatureContext featureContext);
    }
}
