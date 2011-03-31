using System;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public abstract class SpecFlowProjectConfigurationLoader : ISpecFlowProjectConfigurationLoader
    {
        public virtual SpecFlowProjectConfiguration LoadConfiguration(SpecFlowConfigurationHolder configurationHolder, IProjectReference projectReference)
        {
            SpecFlowProjectConfiguration configuration = new SpecFlowProjectConfiguration();

            if (configurationHolder != null && configurationHolder.HasConfiguration)
            {
                ConfigurationSectionHandler specFlowConfigSection = ConfigurationSectionHandler.CreateFromXml(configurationHolder.XmlString);
                if (specFlowConfigSection != null)
                {
                    configuration.GeneratorConfiguration.UpdateFromConfigFile(specFlowConfigSection);
                    configuration.RuntimeConfiguration.UpdateFromConfigFile(specFlowConfigSection);
                }
            }
            configuration.GeneratorConfiguration.GeneratorVersion = GetGeneratorVersion(projectReference);
            return configuration;
        }

        protected abstract Version GetGeneratorVersion(IProjectReference projectReference);
    }
}