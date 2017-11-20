using System;
using System.Globalization;
using System.Text;
using BoDi;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Infrastructure
{
    using System.Diagnostics;

    public interface IContextManager
    {
        TestThreadContext TestThreadContext { get; }
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }
        ScenarioStepContext StepContext { get; }
        StepDefinitionType? CurrentTopLevelStepDefinitionType { get; }

        void InitializeFeatureContext(FeatureInfo featureInfo);
        void CleanupFeatureContext();

        void InitializeScenarioContext(ScenarioInfo scenarioInfo, Action<IObjectContainer> additionalScenarioContextRegistrations);
        void CleanupScenarioContext();

        void InitializeStepContext(StepInfo stepInfo);
        void CleanupStepContext();
    }
}
