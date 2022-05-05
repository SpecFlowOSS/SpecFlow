using System.Threading.Tasks;

namespace TechTalk.SpecFlow
{
    public interface ITestRunner
    {
        /// <summary>
        /// The ID of the parallel test worker processing the current scenario. How the worker ID is obtained is dependent on the test execution framework.
        /// </summary>
        string TestWorkerId { get; }
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }

        void InitializeTestRunner(string testWorkerId);

        Task OnTestRunStartAsync();
        Task OnTestRunEndAsync();

        Task OnFeatureStartAsync(FeatureInfo featureInfo);
        Task OnFeatureEndAsync();

        void OnScenarioInitialize(ScenarioInfo scenarioInfo);
        Task OnScenarioStartAsync();

        Task CollectScenarioErrorsAsync();
        Task OnScenarioEndAsync();

        void SkipScenario();

        Task GivenAsync(string text, string multilineTextArg, Table tableArg, string keyword = null);
        Task WhenAsync(string text, string multilineTextArg, Table tableArg, string keyword = null);
        Task ThenAsync(string text, string multilineTextArg, Table tableArg, string keyword = null);
        Task AndAsync(string text, string multilineTextArg, Table tableArg, string keyword = null);
        Task ButAsync(string text, string multilineTextArg, Table tableArg, string keyword = null);

        void Pending();
    }
}
