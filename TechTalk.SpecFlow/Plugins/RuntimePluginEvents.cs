using System;
using BoDi;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Plugins
{
    public class RuntimePluginEvents
    {
        public event EventHandler<RegisterGlobalDependenciesEventArgs> RegisterGlobalDependencies;
        public event EventHandler<CustomizeGlobalDependenciesEventArgs> CustomizeGlobalDependencies;
        public event EventHandler<ConfigurationDefaultsEventArgs> ConfigurationDefaults;
        public event EventHandler<CustomizeTestThreadDependenciesEventArgs> CustomizeTestThreadDependencies;
        public event EventHandler<CustomizeFeatureDependenciesEventArgs> CustomizeFeatureDependencies;
        public event EventHandler<CustomizeScenarioDependenciesEventArgs> CustomizeScenarioDependencies;
        public event EventHandler<TestRunEndEventArgs> TestRunEnd;

        public void RaiseRegisterGlobalDependencies(ObjectContainer objectContainer)
        {
            RegisterGlobalDependencies?.Invoke(this, new RegisterGlobalDependenciesEventArgs(objectContainer));
        }

        public void RaiseConfigurationDefaults(Configuration.SpecFlowConfiguration specFlowConfiguration)
        {
            ConfigurationDefaults?.Invoke(this, new ConfigurationDefaultsEventArgs(specFlowConfiguration));
        }

        public void RaiseCustomizeGlobalDependencies(ObjectContainer container, SpecFlowConfiguration specFlowConfiguration)
        {
            CustomizeGlobalDependencies?.Invoke(this, new CustomizeGlobalDependenciesEventArgs(container, specFlowConfiguration));
        }

        public void RaiseCustomizeTestThreadDependencies(ObjectContainer testThreadContainer)
        {
            CustomizeTestThreadDependencies?.Invoke(this, new CustomizeTestThreadDependenciesEventArgs(testThreadContainer));
        }

        public void RaiseCustomizeFeatureDependencies(ObjectContainer featureContainer)
        {
            CustomizeFeatureDependencies?.Invoke(this, new CustomizeFeatureDependenciesEventArgs(featureContainer));
        }

        public void RaiseCustomizeScenarioDependencies(ObjectContainer scenarioContainer)
        {
            CustomizeScenarioDependencies?.Invoke(this, new CustomizeScenarioDependenciesEventArgs(scenarioContainer));
        }

        public void RaiseTestRunEnd(ITestRunnerManager testRunnerManager, ObjectContainer container)
        {
            TestRunEnd?.Invoke(this, new TestRunEndEventArgs(testRunnerManager, container));
        }
    }
}