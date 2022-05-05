using System.Threading.Tasks;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public class TestRunner : ITestRunner
    {
        private readonly ITestExecutionEngine _executionEngine;

        public string TestWorkerId { get; private set; }

        public TestRunner(ITestExecutionEngine executionEngine)
        {
            _executionEngine = executionEngine;
        }

        public FeatureContext FeatureContext
        {
            get { return _executionEngine.FeatureContext; }
        }

        public ScenarioContext ScenarioContext
        {
            get { return _executionEngine.ScenarioContext; }
        }

        public async Task OnTestRunStartAsync()
        {
            await _executionEngine.OnTestRunStartAsync();
        }

        public void InitializeTestRunner(string testWorkerId)
        {
            TestWorkerId = testWorkerId;
        }

        public async Task OnFeatureStartAsync(FeatureInfo featureInfo)
        {
            await _executionEngine.OnFeatureStartAsync(featureInfo);
        }

        public async Task OnFeatureEndAsync()
        {
            await _executionEngine.OnFeatureEndAsync();
        }

        public void OnScenarioInitialize(ScenarioInfo scenarioInfo)
        {
            _executionEngine.OnScenarioInitialize(scenarioInfo);
        }

        public async Task OnScenarioStartAsync()
        {
            await _executionEngine.OnScenarioStartAsync();
        }

        public async Task CollectScenarioErrorsAsync()
        {
            await _executionEngine.OnAfterLastStepAsync();
        }

        public async Task OnScenarioEndAsync()
        {
            await _executionEngine.OnScenarioEndAsync();
        }

        public void SkipScenario()
        {
            _executionEngine.OnScenarioSkipped();
        }

        public async Task OnTestRunEndAsync()
        {
            await _executionEngine.OnTestRunEndAsync();
        }

        public async Task GivenAsync(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            await _executionEngine.StepAsync(StepDefinitionKeyword.Given, keyword, text, multilineTextArg, tableArg);
        }

        public async Task WhenAsync(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            await _executionEngine.StepAsync(StepDefinitionKeyword.When, keyword, text, multilineTextArg, tableArg);
        }

        public async Task ThenAsync(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            await _executionEngine.StepAsync(StepDefinitionKeyword.Then, keyword, text, multilineTextArg, tableArg);
        }

        public async Task AndAsync(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            await _executionEngine.StepAsync(StepDefinitionKeyword.And, keyword, text, multilineTextArg, tableArg);
        }

        public async Task ButAsync(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            await _executionEngine.StepAsync(StepDefinitionKeyword.But, keyword, text, multilineTextArg, tableArg);
        }

        public void Pending()
        {
            _executionEngine.Pending();
        }
    }
}
