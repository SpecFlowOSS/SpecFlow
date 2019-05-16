using Google.Protobuf;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public interface IProtobufFileSinkOutput
    {
        bool WriteMessage(IMessage message);
    }
}
