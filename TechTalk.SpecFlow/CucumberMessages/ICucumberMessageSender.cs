using System;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageSender
    {
        void SendTestRunStarted();

        void SendTestCaseStarted(Guid pickleId);
    }
}
