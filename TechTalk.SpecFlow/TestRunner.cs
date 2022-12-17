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

        public Task OnTestRunStartAsync()
        {
            return _executionEngine.OnTestRunStartAsync();
        }

        public void InitializeTestRunner(string testWorkerId)
        {
            TestWorkerId = testWorkerId;
        }

        public Task OnFeatureStartAsync(FeatureInfo featureInfo)
        {
            return _executionEngine.OnFeatureStartAsync(featureInfo);
        }

        public Task OnFeatureEndAsync()
        {
            return _executionEngine.OnFeatureEndAsync();
        }

        public void OnScenarioInitialize(ScenarioInfo scenarioInfo)
        {
            _executionEngine.OnScenarioInitialize(scenarioInfo);
        }

        public Task OnScenarioStartAsync()
        {
            return _executionEngine.OnScenarioStartAsync();
        }

        public Task CollectScenarioErrorsAsync()
        {
            return _executionEngine.OnAfterLastStepAsync();
        }

        public Task OnScenarioEndAsync()
        {
            return _executionEngine.OnScenarioEndAsync();
        }

        public void SkipScenario()
        {
            _executionEngine.OnScenarioSkipped();
        }

        public Task OnTestRunEndAsync()
        {
            return _executionEngine.OnTestRunEndAsync();
        }

        public Task GivenAsync(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            return _executionEngine.StepAsync(StepDefinitionKeyword.Given, keyword, text, multilineTextArg, tableArg);
        }

        public Task WhenAsync(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            return _executionEngine.StepAsync(StepDefinitionKeyword.When, keyword, text, multilineTextArg, tableArg);
        }

        public Task ThenAsync(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            return _executionEngine.StepAsync(StepDefinitionKeyword.Then, keyword, text, multilineTextArg, tableArg);
        }

        public Task AndAsync(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            return _executionEngine.StepAsync(StepDefinitionKeyword.And, keyword, text, multilineTextArg, tableArg);
        }

        public Task ButAsync(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            return _executionEngine.StepAsync(StepDefinitionKeyword.But, keyword, text, multilineTextArg, tableArg);
        }

        public void Pending()
        {
            _executionEngine.Pending();
        }
    }
}
