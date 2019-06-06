using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ITestResultFactory
    {
        IResult<TestResult> BuildPassedResult(ulong durationInNanoseconds);

        IResult<TestResult> BuildFailedResult(ulong durationInNanoseconds, string message);

        IResult<TestResult> BuildAmbiguousResult(ulong durationInNanoseconds, string message);

        IResult<TestResult> BuildPendingResult(ulong durationInNanoseconds, string message);

        IResult<TestResult> BuildSkippedResult(ulong durationInNanoseconds, string message);

        IResult<TestResult> BuildUndefinedResult(ulong durationInNanoseconds, string message);

        IResult<TestResult> BuildFromContext(ScenarioContext scenarioContext, FeatureContext featureContext);
    }
}
