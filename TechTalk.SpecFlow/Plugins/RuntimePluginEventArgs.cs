using System;
using BoDi;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Plugins
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
        public CustomizeGlobalDependenciesEventArgs(ObjectContainer objectContainer, SpecFlowConfiguration specFlowConfiguration) 
            : base(objectContainer)
        {
            SpecFlowConfiguration = specFlowConfiguration;
        }

        public SpecFlowConfiguration SpecFlowConfiguration { get; private set; }
    }

    public class ConfigurationDefaultsEventArgs : EventArgs
    {
        public ConfigurationDefaultsEventArgs(SpecFlowConfiguration specFlowConfiguration)
        {
            SpecFlowConfiguration = specFlowConfiguration;
        }

        public SpecFlowConfiguration SpecFlowConfiguration { get; private set; }
    }

    public class CustomizeTestThreadDependenciesEventArgs : ObjectContainerEventArgs
    {
        public CustomizeTestThreadDependenciesEventArgs(ObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }

    public class CustomizeFeatureDependenciesEventArgs : ObjectContainerEventArgs
    {
        public CustomizeFeatureDependenciesEventArgs(ObjectContainer objectContainer) : base(objectContainer)
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