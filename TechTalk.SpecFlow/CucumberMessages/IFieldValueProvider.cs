using System;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface IFieldValueProvider
    {
        DateTime GetTestRunStartedTime();
        DateTime GetTestCaseStartedTime();
        Guid GetTestCaseStartedPickleId(ScenarioInfo scenarioInfo);
        DateTime GetTestCaseFinishedTime();
        Guid GetTestCaseFinishedPickleId(ScenarioInfo scenarioInfo);
    }
}
