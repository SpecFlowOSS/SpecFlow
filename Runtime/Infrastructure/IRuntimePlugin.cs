using System;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class RuntimePluginEvents
    {
        public event EventHandler<RegisterGlobalDependenciesEventArgs> RegisterGlobalDependencies;
        public event EventHandler<CustomizeGlobalDependenciesEventArgs> CustomizeGlobalDependencies;
        public event EventHandler<ConfigurationDefaultsEventArgs> ConfigurationDefaults;
        public event EventHandler<CustomizeTestThreadDependenciesEventArgs> CustomizeTestThreadDependencies;

        public void RaiseRegisterGlobalDependencies(ObjectContainer objectContainer)
        {
            RegisterGlobalDependencies?.Invoke(this, new RegisterGlobalDependenciesEventArgs(objectContainer));
        }

        public void RaiseConfigurationDefaults(RuntimeConfiguration runtimeConfiguration)
        {
            ConfigurationDefaults?.Invoke(this, new ConfigurationDefaultsEventArgs(runtimeConfiguration));
        }

        public void RaiseCustomizeGlobalDependencies(ObjectContainer container, RuntimeConfiguration runtimeConfiguration)
        {
            CustomizeGlobalDependencies?.Invoke(this, new CustomizeGlobalDependenciesEventArgs(container, runtimeConfiguration));
        }

        public void RaiseCustomizeTestThreadDependencies(ObjectContainer testThreadContainer)
        {
            CustomizeTestThreadDependencies?.Invoke(this, new CustomizeTestThreadDependenciesEventArgs(testThreadContainer));
        }
    }

    public class RuntimePluginParameters
    {
        public string Parameter { get; set; }
    }



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
