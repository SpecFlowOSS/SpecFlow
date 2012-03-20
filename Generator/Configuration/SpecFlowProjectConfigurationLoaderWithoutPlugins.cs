using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class SpecFlowProjectConfigurationLoaderWithoutPlugins : SpecFlowProjectConfigurationLoader
    {
        internal override void UpdateGeneratorConfiguration(SpecFlowProjectConfiguration configuration, ConfigurationSectionHandler specFlowConfigSection)
        {
            //TODO: check usages of loading config without plugins
            configuration.GeneratorConfiguration.UpdateFromConfigFile(specFlowConfigSection);
        }
    }
}