using System;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IRuntimePlugin
    {
        void RegisterDependencies(ObjectContainer container);
        void RegisterCustomizations(ObjectContainer container, RuntimeConfiguration runtimeConfiguration);
        void RegisterConfigurationDefaults(RuntimeConfiguration runtimeConfiguration);
    }

    [Flags]
    public enum PluginType
    {
        Generator = 1,
        Runtime = 2,
        GeneratorAndRuntime = Generator | Runtime
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
