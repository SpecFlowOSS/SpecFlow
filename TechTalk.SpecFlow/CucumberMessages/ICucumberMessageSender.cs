using System;
using Io.Cucumber.Messages;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageSender
    {
        void SendTestRunStarted();

        void SendTestCaseStarted(ScenarioInfo scenarioInfo);

        void SendTestCaseFinished(ScenarioInfo scenarioInfo, TestResult testResult);
    }
}
