using System.Collections.Generic;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public interface IGeneratorConfigurationProvider
    {
        void LoadConfiguration(SpecFlowConfigurationHolder configurationHolder, SpecFlowProjectConfiguration specFlowProjectConfiguration);
        IEnumerable<PluginDescriptor> GetPlugins(SpecFlowConfigurationHolder configurationHolder);
    }

    public static class GeneratorConfigurationProviderExtensions
    {
        public static SpecFlowProjectConfiguration LoadConfiguration(this IGeneratorConfigurationProvider configurationProvider, SpecFlowConfigurationHolder configurationHolder)
        {
            SpecFlowProjectConfiguration configuration = new SpecFlowProjectConfiguration();
            configurationProvider.LoadConfiguration(configurationHolder, configuration);
            return configuration;
        }
    }
}
