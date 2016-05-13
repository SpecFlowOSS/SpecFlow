using System;
using System.Linq;

namespace TechTalk.SpecFlow.Infrastructure
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
        public string Parameter { get; set; }
    }

    public class PluginDescriptor
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public PluginType Type { get; private set; }

        public PluginDescriptor(string name, string path, PluginType type)
        {
            Name = name;
            Path = path;
            Type = type;
        }
    }
}
