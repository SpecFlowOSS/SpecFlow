using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ITestResultPartsFactory
    {
        IResult<TestResult> BuildPassedResult(ulong durationInNanoseconds);
        IResult<TestResult> BuildFailedResult(ulong durationInNanoseconds, ScenarioContext scenarioContext);
        IResult<TestResult> BuildAmbiguousResult(ulong durationInNanoseconds, ScenarioContext scenarioContext);
        IResult<TestResult> BuildPendingResult(ulong durationInNanoseconds, ScenarioContext scenarioContext);
        IResult<TestResult> BuildSkippedResult(ulong durationInNanoseconds);
        IResult<TestResult> BuildUndefinedResult(ulong durationInNanoseconds, ScenarioContext scenarioContext, FeatureContext featureContext);
    }
}