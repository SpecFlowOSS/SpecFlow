using MiniDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.Generator
{
    public class GeneratorContainerBuilder
    {
        public static IObjectContainer CreateContainer(SpecFlowConfigurationHolder configurationHolder)
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

            return container;
        }

        private static void RegisterDefaults(ObjectContainer container)
        {
            container.RegisterTypeAs<SpecFlowProjectConfigurationLoader, ISpecFlowProjectConfigurationLoader>();
            container.RegisterTypeAs<InProcGeneratorInfoProvider, IGeneratorInfoProvider>();
            container.RegisterTypeAs<TestGenerator, ITestGenerator>();
            container.RegisterTypeAs<TestHeaderWriter, ITestHeaderWriter>();
            container.RegisterTypeAs<TestUpToDateChecker, ITestUpToDateChecker>();
        }
    }
}