using System;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public class GeneratorPluginEvents
    {
        public event EventHandler<RegisterDependenciesEventArgs> RegisterDependencies;
        public event EventHandler<CustomizeDependenciesEventArgs> CustomizeDependencies;
        public event EventHandler<ConfigurationDefaultsEventArgs> ConfigurationDefaults;

        public void RaiseRegisterDependencies(ObjectContainer objectContainer)
        {
            RegisterDependencies?.Invoke(this, new RegisterDependenciesEventArgs(objectContainer));
        }

        public void RaiseConfigurationDefaults(SpecFlowProjectConfiguration specFlowProjectConfiguration)
        {
            ConfigurationDefaults?.Invoke(this, new ConfigurationDefaultsEventArgs(specFlowProjectConfiguration));
        }

        public void RaiseCustomizeDependencies(ObjectContainer container, SpecFlowProjectConfiguration specFlowProjectConfiguration)
        {
            CustomizeDependencies?.Invoke(this, new CustomizeDependenciesEventArgs(container, specFlowProjectConfiguration));
        }
    }
}
