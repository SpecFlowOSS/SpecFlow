using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly:GeneratorPlugin(typeof(TechTalk.SpecFlow.NUnit.Generator.SpecFlowPlugin.GeneratorPlugin))]

namespace TechTalk.SpecFlow.NUnit.Generator.SpecFlowPlugin
{
    public class GeneratorPlugin : IGeneratorPlugin
    {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            unitTestProviderConfiguration.UseUnitTestProvider("nunit");
        }
    }
}
