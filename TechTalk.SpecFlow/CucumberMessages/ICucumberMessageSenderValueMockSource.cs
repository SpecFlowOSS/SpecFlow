using System;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageSenderValueMockSource
    {
        DateTime? GetTestRunStartedTimeFromEnvironmentVariableOrNull();
        DateTime? GetTestCaseStartedTimeFromEnvironmentVariableOrNull();
        Guid? GetTestCaseStartedPickleIdFromEnvironmentVariableOrNull();
        DateTime? GetTestCaseFinishedTimeFromEnvironmentVariableOrNull();
        Guid? GetTestCaseFinishedPickleIdFromEnvironmentVariableOrNull();
    }
}
