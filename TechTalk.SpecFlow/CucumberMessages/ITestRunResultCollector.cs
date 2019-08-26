using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ITestRunResultCollector
    {
        void StartCollecting();

        void CollectTestResultForScenario(ScenarioInfo scenarioInfo, TestResult testResult);

        IResult<TestRunResult> StopCollecting();
    }
}
