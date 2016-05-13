using System;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.Plugins
{
    public class RegisterDependenciesEventArgs : EventArgs
    {
        public RegisterDependenciesEventArgs(ObjectContainer objectContainer)
        {
            ObjectContainer = objectContainer;
        }

        public ObjectContainer ObjectContainer { get; private set; }
    }

    public class CustomizeDependenciesEventArgs : EventArgs
    {
        public CustomizeDependenciesEventArgs(ObjectContainer objectContainer, SpecFlowProjectConfiguration specFlowProjectConfiguration)
        {
            ObjectContainer = objectContainer;
            this.SpecFlowProjectConfiguration = specFlowProjectConfiguration;
        }

        public ObjectContainer ObjectContainer { get; private set; }
        public SpecFlowProjectConfiguration SpecFlowProjectConfiguration { get; private set; }
    }


    public class ConfigurationDefaultsEventArgs : EventArgs
    {
        public ConfigurationDefaultsEventArgs(SpecFlowProjectConfiguration specFlowProjectConfiguration)
        {
            this.SpecFlowProjectConfiguration = specFlowProjectConfiguration;
        }

        public SpecFlowProjectConfiguration SpecFlowProjectConfiguration { get; private set; }
    }
}
