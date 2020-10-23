using SpecFlow.ExternalData.SpecFlowPlugin;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly:GeneratorPlugin(typeof(ExternalDataGeneratorPlugin))]

namespace SpecFlow.ExternalData.SpecFlowPlugin
{
    public class ExternalDataGeneratorPlugin : IGeneratorPlugin
    {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            generatorPluginEvents.RegisterDependencies += (sender, args) =>
            {
                args.ObjectContainer.RegisterTypeAs<ExternalDataTestGenerator, ITestGenerator>();
                args.ObjectContainer.RegisterTypeAs<ExternalDataFeaturePatcher, IExternalDataFeaturePatcher>();
                args.ObjectContainer.RegisterTypeAs<TestDataProvider, ITestDataProvider>();
            };
        }
    }
}
