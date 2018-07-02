using System.Collections.Generic;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class GeneratorConfigurationProvider : IGeneratorConfigurationProvider
    {
        private readonly IConfigurationLoader _configurationLoader;

        public GeneratorConfigurationProvider(IConfigurationLoader configurationLoader)
        {
            _configurationLoader = configurationLoader;
        }

        public virtual SpecFlowConfiguration LoadConfiguration(SpecFlowConfiguration specFlowConfiguration, SpecFlowConfigurationHolder specFlowConfigurationHolder)
        {
            return _configurationLoader.Load(specFlowConfiguration, specFlowConfigurationHolder);
        }

        public SpecFlowConfiguration LoadConfiguration(SpecFlowConfiguration specFlowConfiguration)
        {
            return _configurationLoader.Load(specFlowConfiguration);
        }
        
        internal virtual void UpdateConfiguration(SpecFlowProjectConfiguration configuration, ConfigurationSectionHandler specFlowConfigSection)
        {
            configuration.SpecFlowConfiguration = _configurationLoader.Update(configuration.SpecFlowConfiguration, specFlowConfigSection);
        }

    }
}