using MiniDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator
{
    public class GeneratorContainerBuilder
    {
        public static IObjectContainer CreateContainer(SpecFlowConfigurationHolder configurationHolder)
        {
            ConfigurationSectionHandler specFlowConfigSection = null;
            if (configurationHolder != null && configurationHolder.HasConfiguration)
                specFlowConfigSection = ConfigurationSectionHandler.CreateFromXml(configurationHolder.XmlString);

            GeneratorConfiguration generatorConfiguration = new GeneratorConfiguration();
            if (specFlowConfigSection != null)
                generatorConfiguration.UpdateFromConfigFile(specFlowConfigSection);

            var container = new ObjectContainer();

            container.RegisterInstanceAs(generatorConfiguration);

            RegisterDefaults(container);

            if (specFlowConfigSection != null && specFlowConfigSection.Dependencies != null)
                container.RegisterFromConfiguration(specFlowConfigSection.Dependencies);

            return container;
        }

        private static void RegisterDefaults(ObjectContainer container)
        {
            container.RegisterTypeAs<TestGenerator, ITestGenerator>();
            container.RegisterTypeAs<TestHeaderWriter, ITestHeaderWriter>();
        }
    }
}