using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IGeneratorPluginLocator
    {
        string LocatePluginAssembly(PluginDescriptor pluginDescriptor);
    }
}
