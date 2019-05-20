namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageSender
    {
        void SendTestRunStarted();

        void SendTestCaseStarted(string pickleId);
    }
}
