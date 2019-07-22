using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ITestResultFactory
    {
        IResult<TestResult> BuildFromContext(ScenarioContext scenarioContext, FeatureContext featureContext);
    }
}
