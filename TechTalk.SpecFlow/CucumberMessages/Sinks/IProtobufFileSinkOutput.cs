using Google.Protobuf;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public interface IProtobufFileSinkOutput
    {
        IResult WriteMessage(Envelope message);
    }
}
