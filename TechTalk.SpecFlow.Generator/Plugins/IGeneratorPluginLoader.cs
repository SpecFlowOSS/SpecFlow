using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public interface IGeneratorPluginLoader
    {
        IGeneratorPlugin LoadPlugin(PluginDescriptor pluginDescriptor);
    }
}
