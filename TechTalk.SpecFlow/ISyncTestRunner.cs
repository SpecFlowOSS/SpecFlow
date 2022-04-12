using System;

namespace TechTalk.SpecFlow
{
    public interface ISyncTestRunner
    {
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }

        void OnTestRunStart();
        void OnTestRunEnd();

        void OnFeatureStart(FeatureInfo featureInfo);
        void OnFeatureEnd();

        void OnScenarioInitialize(ScenarioInfo scenarioInfo);
        void OnScenarioStart();

        void CollectScenarioErrors();
        void OnScenarioEnd();

        void SkipScenario();

        void Given(string text, string multilineTextArg, Table tableArg, string keyword = null);
        void When(string text, string multilineTextArg, Table tableArg, string keyword = null);
        void Then(string text, string multilineTextArg, Table tableArg, string keyword = null);
        void And(string text, string multilineTextArg, Table tableArg, string keyword = null);
        void But(string text, string multilineTextArg, Table tableArg, string keyword = null);

        void Pending();
    }
}
