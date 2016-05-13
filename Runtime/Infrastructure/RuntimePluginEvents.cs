using System;
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
}