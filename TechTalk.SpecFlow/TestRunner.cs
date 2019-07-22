using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public class TestRunner : ITestRunner
    {
        private readonly ITestExecutionEngine _executionEngine;

        public int ThreadId { get; private set; }

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

        public void OnTestRunStart()
        {
            _executionEngine.OnTestRunStart();
        }

        public void InitializeTestRunner(int threadId)
        {
            ThreadId = threadId;
        }

        public void OnFeatureStart(FeatureInfo featureInfo)
        {
            _executionEngine.OnFeatureStart(featureInfo);
        }

        public void OnFeatureEnd()
        {
            _executionEngine.OnFeatureEnd();
        }

        public void OnScenarioInitialize(ScenarioInfo scenarioInfo)
        {
            _executionEngine.OnScenarioInitialize(scenarioInfo);
        }

        public void OnScenarioStart()
        {
            _executionEngine.OnScenarioStart();
        }

        public void CollectScenarioErrors()
        {
            _executionEngine.OnAfterLastStep();
        }

        public void OnScenarioEnd()
        {
            _executionEngine.OnScenarioEnd();
        }

        public void SkipScenario()
        {
            _executionEngine.OnScenarioSkipped();
        }

        public void OnTestRunEnd()
        {
            _executionEngine.OnTestRunEnd();
        }

        public void Given(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            _executionEngine.Step(StepDefinitionKeyword.Given, keyword, text, multilineTextArg, tableArg);
        }

        public void When(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            _executionEngine.Step(StepDefinitionKeyword.When, keyword, text, multilineTextArg, tableArg);
        }

        public void Then(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            _executionEngine.Step(StepDefinitionKeyword.Then, keyword, text, multilineTextArg, tableArg);
        }

        public void And(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            _executionEngine.Step(StepDefinitionKeyword.And, keyword, text, multilineTextArg, tableArg);
        }

        public void But(string text, string multilineTextArg, Table tableArg, string keyword = null)
        {
            _executionEngine.Step(StepDefinitionKeyword.But, keyword, text, multilineTextArg, tableArg);
        }

        public void Pending()
        {
            _executionEngine.Pending();
        }
    }
}
