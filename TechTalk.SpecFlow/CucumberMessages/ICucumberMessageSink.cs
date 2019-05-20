using Google.Protobuf;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageSink
    {
        void SendMessage(IMessage message);
    }
}
