using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IGeneratorPlugin
    {
        void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters, UnitTestProviderConfiguration unitTestProviderConfiguration);
    }
}