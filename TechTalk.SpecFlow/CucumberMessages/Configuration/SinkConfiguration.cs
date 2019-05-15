using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages.Configuration
{
    public class SinkConfiguration : ISinkConfiguration
    {
        private readonly SinkConfigurationEntry _sinkConfigurationEntry;

        public SinkConfiguration(SinkConfigurationEntry sinkConfigurationEntry)
        {
            _sinkConfigurationEntry = sinkConfigurationEntry;
        }

        public string TypeName => _sinkConfigurationEntry?.TypeName;

        public Result<string> GetString(string name)
        {
            if (_sinkConfigurationEntry?.ConfigurationValues?[name] is string result)
            {
                return Result<string>.Success(result);
            }

            return Result<string>.Failure();
        }
    }
}
