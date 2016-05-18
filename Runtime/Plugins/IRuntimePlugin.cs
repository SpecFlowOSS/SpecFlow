using System;
using System.Linq;

namespace TechTalk.SpecFlow.Plugins
{
    public interface IRuntimePlugin
    {
        void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters);
    }

    [Flags]
    public enum PluginType
    {
        Generator = 1,
        Runtime = 2,
        GeneratorAndRuntime = Generator | Runtime
    }

    public class RuntimePluginParameters
    {
        public string Parameters { get; set; }
    }

    public class PluginDescriptor
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public PluginType Type { get; private set; }
        public string Parameters { get; private set; }

        public PluginDescriptor(string name, string path, PluginType type, string parameters)
        {
            Name = name;
            Path = path;
            Type = type;
            Parameters = parameters;
        }
    }
}
