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
            var configLoader = new RuntimeSpecFlowProjectConfigurationLoader();
            var specFlowConfiguration = configLoader.LoadConfiguration(configurationHolder, new AppDomainProjectReference());

            var container = new ObjectContainer();

            container.RegisterInstanceAs(specFlowConfiguration);
            container.RegisterInstanceAs(specFlowConfiguration.GeneratorConfiguration);
            container.RegisterInstanceAs(specFlowConfiguration.RuntimeConfiguration);

            RegisterDefaults(container);

            if (specFlowConfiguration.GeneratorConfiguration.CustomDependencies != null)
                container.RegisterFromConfiguration(specFlowConfiguration.GeneratorConfiguration.CustomDependencies);

            return container;
        }

        private static void RegisterDefaults(ObjectContainer container)
        {
            container.RegisterTypeAs<TestGenerator, ITestGenerator>();
            container.RegisterTypeAs<TestHeaderWriter, ITestHeaderWriter>();
            container.RegisterTypeAs<TestUpToDateChecker, ITestUpToDateChecker>();
        }
    }
}