using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages.Configuration;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public interface ISinkFactory
    {
        Result<ICucumberMessageSink> FromConfiguration(ISinkConfiguration sinkConfiguration);
    }
}