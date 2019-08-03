using Io.Cucumber.Messages;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageSink
    {
        void SendMessage(Envelope message);
    }
}
