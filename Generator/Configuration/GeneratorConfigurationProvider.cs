using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class GeneratorConfigurationProvider : IGeneratorConfigurationProvider
    {
        public virtual void LoadConfiguration(SpecFlowConfigurationHolder configurationHolder, SpecFlowProjectConfiguration configuration)
        {
            try
            {
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
            }
            catch(Exception ex)
            {
                throw new ConfigurationErrorsException("SpecFlow configuration error", ex);
            }
        }

        public IEnumerable<PluginDescriptor> GetPlugins(SpecFlowConfigurationHolder configurationHolder)
        {
            try
            {
                if (configurationHolder != null && configurationHolder.HasConfiguration)
                {
                    ConfigurationSectionHandler section = ConfigurationSectionHandler.CreateFromXml(configurationHolder.XmlString);
                    if (section != null && section.Plugins != null)
                    {
                        return section.Plugins.Select(pce => pce.ToPluginDescriptor());
                    }
                }

                return Enumerable.Empty<PluginDescriptor>();
            }
            catch(Exception ex)
            {
                throw new ConfigurationErrorsException("SpecFlow configuration error", ex);
            }
        }

        internal virtual void UpdateRuntimeConfiguration(SpecFlowProjectConfiguration configuration, ConfigurationSectionHandler specFlowConfigSection)
        {
            configuration.RuntimeConfiguration.LoadConfiguration(specFlowConfigSection);
        }

        internal virtual void UpdateGeneratorConfiguration(SpecFlowProjectConfiguration configuration, ConfigurationSectionHandler specFlowConfigSection)
        {
            configuration.GeneratorConfiguration.UpdateFromConfigFile(specFlowConfigSection);
        }
    }
}