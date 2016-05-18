using System;
using BoDi;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Infrastructure
{
    public abstract class ObjectContainerEventArgs : EventArgs
    {
        protected ObjectContainerEventArgs(ObjectContainer objectContainer)
        {
            ObjectContainer = objectContainer;
        }

        public ObjectContainer ObjectContainer { get; private set; }
    }

    public class RegisterGlobalDependenciesEventArgs : ObjectContainerEventArgs
    {
        public RegisterGlobalDependenciesEventArgs(ObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }

    public class CustomizeGlobalDependenciesEventArgs : ObjectContainerEventArgs
    {
        public CustomizeGlobalDependenciesEventArgs(ObjectContainer objectContainer, RuntimeConfiguration runtimeConfiguration) 
            : base(objectContainer)
        {
            this.RuntimeConfiguration = runtimeConfiguration;
        }

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

    public class CustomizeTestThreadDependenciesEventArgs : ObjectContainerEventArgs
    {
        public CustomizeTestThreadDependenciesEventArgs(ObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }

    public class CustomizeScenarioDependenciesEventArgs : ObjectContainerEventArgs
    {
        public CustomizeScenarioDependenciesEventArgs(ObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }   
}