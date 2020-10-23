using System;
using BoDi;

namespace TechTalk.SpecFlow.Plugins
{
    public class RuntimePluginTestExecutionLifecycleEvents
    {
        public event EventHandler<RuntimePluginBeforeTestRunEventArgs> BeforeTestRun;

        public event EventHandler<RuntimePluginAfterTestRunEventArgs> AfterTestRun;

        public event EventHandler<RuntimePluginBeforeFeatureEventArgs> BeforeFeature;

        public event EventHandler<RuntimePluginAfterFeatureEventArgs> AfterFeature;

        public event EventHandler<RuntimePluginBeforeScenarioEventArgs> BeforeScenario;

        public event EventHandler<RuntimePluginAfterScenarioEventArgs> AfterScenario;

        public event EventHandler<RuntimePluginBeforeStepEventArgs> BeforeStep;

        public event EventHandler<RuntimePluginAfterStepEventArgs> AfterStep;

        
        public void RaiseBeforeTestRun(IObjectContainer objectContainer)
        {
            BeforeTestRun?.Invoke(this, new RuntimePluginBeforeTestRunEventArgs(objectContainer));
        }

        public void RaiseAfterTestRun(IObjectContainer objectContainer)
        {
            AfterTestRun?.Invoke(this, new RuntimePluginAfterTestRunEventArgs(objectContainer));
        }

        public void RaiseBeforeFeature(IObjectContainer objectContainer)
        {
            BeforeFeature?.Invoke(this, new RuntimePluginBeforeFeatureEventArgs(objectContainer));
        }

        public void RaiseAfterFeature(IObjectContainer objectContainer)
        {
            AfterFeature?.Invoke(this, new RuntimePluginAfterFeatureEventArgs(objectContainer));
        }

        public void RaiseBeforeScenario(IObjectContainer objectContainer)
        {
            BeforeScenario?.Invoke(this, new RuntimePluginBeforeScenarioEventArgs(objectContainer));
        }

        public void RaiseAfterScenario(IObjectContainer objectContainer)
        {
            AfterScenario?.Invoke(this, new RuntimePluginAfterScenarioEventArgs(objectContainer));
        }

        public void RaiseBeforeStep(IObjectContainer objectContainer)
        {
            BeforeStep?.Invoke(this, new RuntimePluginBeforeStepEventArgs(objectContainer));
        }

        public void RaiseAfterStep(IObjectContainer objectContainer)
        {
            AfterStep?.Invoke(this, new RuntimePluginAfterStepEventArgs(objectContainer));
        }
    }
}
