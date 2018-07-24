using TechTalk.SpecFlow.TestProjectGenerator.NewApi;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi.Driver;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi._5_TestRun;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExecutionSteps
    {
        private readonly SolutionDriver _solutionDriver;
        private readonly VSTestExecutionDriver _vsTestExecution;
        private readonly TestRunConfiguration _testRunConfiguration;

        public ExecutionSteps(SolutionDriver solutionDriver, VSTestExecutionDriver vsTestExecution, TestRunConfiguration testRunConfiguration)
        {
            _solutionDriver = solutionDriver;
            _vsTestExecution = vsTestExecution;
            _testRunConfiguration = testRunConfiguration;
        }

        [When(@"I execute the tests")]
        public void WhenIExecuteTheTests()
        {
            _solutionDriver.CompileSolution();
            _solutionDriver.CheckSolutionShouldHaveCompiled();
            _vsTestExecution.ExecuteTests();
        }

        [When(@"I execute the tests tagged with '@(.+)'")]
        public void WhenIExecuteTheTestsTaggedWithTag(string tag)
        {
            _solutionDriver.CompileSolution();
            _solutionDriver.CheckSolutionShouldHaveCompiled();
            if (_testRunConfiguration.UnitTestProvider == TestProjectGenerator.UnitTestProvider.XUnit)
                _vsTestExecution.Filter = $"Category={tag}";
            else
                _vsTestExecution.Filter = $"TestCategory={tag}";
            _vsTestExecution.ExecuteTests();
        }
    }
}
