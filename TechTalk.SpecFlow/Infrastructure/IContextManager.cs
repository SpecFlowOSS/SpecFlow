using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IContextManager
    {
        TestThreadContext TestThreadContext { get; }
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }
        ScenarioStepContext StepContext { get; }
        StepDefinitionType? CurrentTopLevelStepDefinitionType { get; }

        void InitializeFeatureContext(FeatureInfo featureInfo);
        void CleanupFeatureContext();

        void InitializeScenarioContext(ScenarioInfo scenarioInfo);
        void CleanupScenarioContext();

        void InitializeStepContext(StepInfo stepInfo);
        void CleanupStepContext();
    }
}
