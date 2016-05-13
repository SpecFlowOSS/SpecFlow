using System;
using BoDi;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class RegisterGlobalDependenciesEventArgs : EventArgs
    {
        public RegisterGlobalDependenciesEventArgs(ObjectContainer objectContainer)
        {
            ObjectContainer = objectContainer;
        }

        public ObjectContainer ObjectContainer { get; private set; }
    }

    public class CustomizeGlobalDependenciesEventArgs : EventArgs
    {
        public CustomizeGlobalDependenciesEventArgs(ObjectContainer objectContainer, RuntimeConfiguration runtimeConfiguration)
        {
            ObjectContainer = objectContainer;
            this.RuntimeConfiguration = runtimeConfiguration;
        }

        public ObjectContainer ObjectContainer { get; private set; }
        public RuntimeConfiguration RuntimeConfiguration { get; private set; }
    }


    public class ConfigurationDefaultsEventArgs : EventArgs
    {
        public ConfigurationDefaultsEventArgs(RuntimeConfiguration runtimeConfiguration)
        {
            this.RuntimeConfiguration = runtimeConfiguration;
        }

        public RuntimeConfiguration RuntimeConfiguration { get; private set; }
    }

    public class CustomizeTestThreadDependenciesEventArgs : EventArgs
    {
        public CustomizeTestThreadDependenciesEventArgs(ObjectContainer objectContainer)
        {
            ObjectContainer = objectContainer;
        }

        public ObjectContainer ObjectContainer { get; private set; }

    }   
}