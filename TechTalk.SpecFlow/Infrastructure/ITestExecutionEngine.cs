using System.Threading.Tasks;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface ITestExecutionEngine
    {
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }

        Task OnTestRunStartAsync();
        Task OnTestRunEndAsync();

        Task OnFeatureStartAsync(FeatureInfo featureInfo);
        Task OnFeatureEndAsync();

        void OnScenarioInitialize(ScenarioInfo scenarioInfo);
        Task OnScenarioStartAsync();
        Task OnAfterLastStepAsync();
        Task OnScenarioEndAsync();

        void OnScenarioSkipped();

        Task StepAsync(StepDefinitionKeyword stepDefinitionKeyword, string keyword, string text, string multilineTextArg, Table tableArg);

        void Pending();
    }
}
