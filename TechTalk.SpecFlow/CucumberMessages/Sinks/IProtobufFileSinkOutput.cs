using Google.Protobuf;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public interface IProtobufFileSinkOutput
    {
        Result WriteMessage(IMessage message);
    }
}
