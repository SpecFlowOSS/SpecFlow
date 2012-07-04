using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ITestExecutionEngine
    {
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }

        void Initialize(Assembly[] bindingAssemblies);

        void OnFeatureStart(FeatureInfo featureInfo);
        void OnFeatureEnd();
        void OnScenarioStart(ScenarioInfo scenarioInfo);
        void OnAfterLastStep();
        void OnScenarioEnd();
        void OnTestRunEnd();

        void Step(StepDefinitionKeyword stepDefinitionKeyword, string keyword, string text, string multilineTextArg, Table tableArg);

        void Pending();
    }
}
