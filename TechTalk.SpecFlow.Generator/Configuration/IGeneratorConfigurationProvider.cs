using System.Collections.Generic;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public interface IGeneratorConfigurationProvider
    {
        SpecFlowConfiguration LoadConfiguration(SpecFlowConfiguration specFlowConfiguration, SpecFlowConfigurationHolder specFlowConfigurationHolder);
        SpecFlowConfiguration LoadConfiguration(SpecFlowConfiguration specFlowConfiguration);
        IEnumerable<PluginDescriptor> GetPlugins(SpecFlowConfiguration specFlowConfiguration, SpecFlowConfigurationHolder specFlowConfigurationHolder);
    }

    public static class GeneratorConfigurationProviderExtensions
    {
        public static SpecFlowProjectConfiguration LoadConfiguration(this IGeneratorConfigurationProvider configurationProvider, SpecFlowConfigurationHolder configurationHolder)
        {
            SpecFlowProjectConfiguration configuration = new SpecFlowProjectConfiguration();
            configuration.SpecFlowConfiguration = configurationProvider.LoadConfiguration(configuration.SpecFlowConfiguration, configurationHolder);

            return configuration;
        }
    }
}
