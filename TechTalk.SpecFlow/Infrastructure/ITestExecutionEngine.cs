using System;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ITestExecutionEngine
    {
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }

        void OnTestRunStart();
        void OnTestRunEnd();

        void OnFeatureStart(FeatureInfo featureInfo);
        void OnFeatureEnd();

        void OnScenarioStart(ScenarioInfo scenarioInfo, Action<IObjectContainer> additionalScenarioContextRegistrations);
        void OnAfterLastStep();
        void OnScenarioEnd();

        void Step(StepDefinitionKeyword stepDefinitionKeyword, string keyword, string text, string multilineTextArg, Table tableArg);

        void Pending();
    }
}
