using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages.Configuration;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public class ProtobufFileSinkConfigurationFactory
    {
        public Result<ProtobufFileSinkConfiguration> FromSinkConfiguration(ISinkConfiguration sinkConfiguration)
        {
            var targetFilePathResult = sinkConfiguration.GetString(nameof(ProtobufFileSinkConfiguration.TargetFilePath));
            if (!(targetFilePathResult is Success<string> success))
            {
                return Result<ProtobufFileSinkConfiguration>.Failure();
            }

            var protobufFileSinkConfiguration = new ProtobufFileSinkConfiguration(success.Result);
            return new Success<ProtobufFileSinkConfiguration>(protobufFileSinkConfiguration);
        }
    }
}
