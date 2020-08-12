using System;
using BoDi;

namespace TechTalk.SpecFlow.Plugins
{
    public abstract class RuntimePluginTestExecutionLifecycleEventArgs : EventArgs
    {
        protected RuntimePluginTestExecutionLifecycleEventArgs(IObjectContainer objectContainer)
        {
            ObjectContainer = objectContainer;
        }

        public IObjectContainer ObjectContainer { get; private set; }
    }


    public class RuntimePluginBeforeTestRunEventArgs : RuntimePluginTestExecutionLifecycleEventArgs
    {
        public RuntimePluginBeforeTestRunEventArgs(IObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }

    public class RuntimePluginAfterTestRunEventArgs : RuntimePluginTestExecutionLifecycleEventArgs
    {
        public RuntimePluginAfterTestRunEventArgs(IObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }

    public class RuntimePluginBeforeFeatureEventArgs : RuntimePluginTestExecutionLifecycleEventArgs
    {
        public RuntimePluginBeforeFeatureEventArgs(IObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }

    public class RuntimePluginAfterFeatureEventArgs : RuntimePluginTestExecutionLifecycleEventArgs
    {
        public RuntimePluginAfterFeatureEventArgs(IObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }

    public class RuntimePluginBeforeScenarioEventArgs : RuntimePluginTestExecutionLifecycleEventArgs
    {
        public RuntimePluginBeforeScenarioEventArgs(IObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }

    public class RuntimePluginAfterScenarioEventArgs : RuntimePluginTestExecutionLifecycleEventArgs
    {
        public RuntimePluginAfterScenarioEventArgs(IObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }

    public class RuntimePluginBeforeStepEventArgs : RuntimePluginTestExecutionLifecycleEventArgs
    {
        public RuntimePluginBeforeStepEventArgs(IObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }

    public class RuntimePluginAfterStepEventArgs : RuntimePluginTestExecutionLifecycleEventArgs
    {
        public RuntimePluginAfterStepEventArgs(IObjectContainer objectContainer) : base(objectContainer)
        {
        }
    }
}