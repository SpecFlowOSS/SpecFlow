using System;
using Io.Cucumber.Messages;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageSender
    {
        void SendTestRunStarted();

        void SendTestCaseStarted(Guid pickleId);

        void SendTestCaseFinished(Guid pickleId, TestResult testResult);
    }
}
