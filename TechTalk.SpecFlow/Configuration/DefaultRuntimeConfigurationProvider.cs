namespace TechTalk.SpecFlow.Configuration
{
    public class DefaultRuntimeConfigurationProvider : IRuntimeConfigurationProvider
    {
        private readonly IConfigurationLoader _configurationLoader;

        public DefaultRuntimeConfigurationProvider(IConfigurationLoader configurationLoader)
        {
            _configurationLoader = configurationLoader;
        }

        public SpecFlowConfiguration LoadConfiguration(SpecFlowConfiguration specFlowConfiguration)
        {
            return _configurationLoader.Load(specFlowConfiguration);
        }
    }
}