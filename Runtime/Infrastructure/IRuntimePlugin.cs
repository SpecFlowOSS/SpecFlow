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

    public class PluginDescriptor
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public PluginDescriptor(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
