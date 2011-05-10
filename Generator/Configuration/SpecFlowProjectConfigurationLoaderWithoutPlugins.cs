using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class SpecFlowProjectConfigurationLoaderWithoutPlugins : SpecFlowProjectConfigurationLoader
    {
        internal override void UpdateGeneratorConfiguration(SpecFlowProjectConfiguration configuration, ConfigurationSectionHandler specFlowConfigSection)
        {
            configuration.GeneratorConfiguration.UpdateFromConfigFile(specFlowConfigSection, false);
        }
    }
}