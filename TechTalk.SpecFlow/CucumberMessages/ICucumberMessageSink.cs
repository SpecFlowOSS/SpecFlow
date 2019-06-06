using Io.Cucumber.Messages;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageSink
    {
        void SendMessage(Wrapper message);
    }
}
