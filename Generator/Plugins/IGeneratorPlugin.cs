using BoDi;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IGeneratorPlugin
    {
        void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters);

        //void RegisterDependencies(ObjectContainer container);
        //void RegisterCustomizations(ObjectContainer container, SpecFlowProjectConfiguration specFlowProjectConfiguration);
        //void RegisterConfigurationDefaults(SpecFlowProjectConfiguration specFlowConfiguration);
    }
}