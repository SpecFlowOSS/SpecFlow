using System;
using System.Configuration;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class SpecFlowProjectConfigurationLoader : ISpecFlowProjectConfigurationLoader
    {
        public virtual SpecFlowProjectConfiguration LoadConfiguration(SpecFlowConfigurationHolder configurationHolder)
        {
            try
            {
                SpecFlowProjectConfiguration configuration = new SpecFlowProjectConfiguration();

                if (configurationHolder != null && configurationHolder.HasConfiguration)
                {
                    ConfigurationSectionHandler specFlowConfigSection =
                        ConfigurationSectionHandler.CreateFromXml(configurationHolder.XmlString);
                    if (specFlowConfigSection != null)
                    {
                        UpdateGeneratorConfiguration(configuration, specFlowConfigSection);
                        UpdateRuntimeConfiguration(configuration, specFlowConfigSection);
                    }
                }
                return configuration;
            }
            catch(Exception ex)
            {
                throw new ConfigurationErrorsException("SpecFlow configuration error", ex);
            }
        }

        internal virtual void UpdateRuntimeConfiguration(SpecFlowProjectConfiguration configuration, ConfigurationSectionHandler specFlowConfigSection)
        {
            configuration.RuntimeConfiguration.UpdateFromConfigFile(specFlowConfigSection);
        }

        internal virtual void UpdateGeneratorConfiguration(SpecFlowProjectConfiguration configuration, ConfigurationSectionHandler specFlowConfigSection)
        {
            configuration.GeneratorConfiguration.UpdateFromConfigFile(specFlowConfigSection, true);
        }
    }
}