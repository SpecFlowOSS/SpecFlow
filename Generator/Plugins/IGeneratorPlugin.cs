namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IGeneratorPlugin
    {
        void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters);
    }
}