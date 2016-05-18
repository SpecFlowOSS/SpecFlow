using BoDi;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IGeneratorPlugin
    {
        void RegisterDependencies(ObjectContainer container);
        void RegisterCustomizations(ObjectContainer container, SpecFlowProjectConfiguration generatorConfiguration);
        void RegisterConfigurationDefaults(SpecFlowProjectConfiguration specFlowConfiguration);
    }
}