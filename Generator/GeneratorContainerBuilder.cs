using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator
{
    public class GeneratorContainerBuilder
    {
        internal static DefaultDependencyProvider DefaultDependencyProvider = new DefaultDependencyProvider();

        public static IObjectContainer CreateContainer(SpecFlowConfigurationHolder configurationHolder, ProjectSettings projectSettings)
        {
            var container = new ObjectContainer();

            RegisterDefaults(container);

            var specFlowConfiguration = container.Resolve<ISpecFlowProjectConfigurationLoader>()
                .LoadConfiguration(configurationHolder);

            if (specFlowConfiguration.GeneratorConfiguration.CustomDependencies != null)
                container.RegisterFromConfiguration(specFlowConfiguration.GeneratorConfiguration.CustomDependencies);

            container.RegisterInstanceAs(specFlowConfiguration);
            container.RegisterInstanceAs(specFlowConfiguration.GeneratorConfiguration);
            container.RegisterInstanceAs(specFlowConfiguration.RuntimeConfiguration);

            var generatorInfo = container.Resolve<IGeneratorInfoProvider>().GetGeneratorInfo();
            container.RegisterInstanceAs(generatorInfo);

            container.RegisterInstanceAs(projectSettings);

            container.RegisterInstanceAs(container.Resolve<CodeDomHelper>(projectSettings.ProjectPlatformSettings.Language));

            if (specFlowConfiguration.GeneratorConfiguration.GeneratorUnitTestProvider != null)
                container.RegisterInstanceAs(container.Resolve<IUnitTestGeneratorProvider>(specFlowConfiguration.GeneratorConfiguration.GeneratorUnitTestProvider));

            return container;
        }

        private static void RegisterDefaults(ObjectContainer container)
        {
            DefaultDependencyProvider.RegisterDefaults(container);
        }
    }
}