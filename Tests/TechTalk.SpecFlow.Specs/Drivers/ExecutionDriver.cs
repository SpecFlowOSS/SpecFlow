using System.Threading.Tasks;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class ExecutionDriver
    {
        private readonly SolutionDriver _solutionDriver;
        private readonly VSTestExecutionDriver _vsTestExecutionDriver;
        private readonly TestRunConfiguration _testRunConfiguration;

        public ExecutionDriver(SolutionDriver solutionDriver, VSTestExecutionDriver vsTestExecutionDriver, TestRunConfiguration testRunConfiguration)
        {
            _solutionDriver = solutionDriver;
            _vsTestExecutionDriver = vsTestExecutionDriver;
            _testRunConfiguration = testRunConfiguration;
        }

        public void ExecuteTestsWithTag(string tag)
        {
            if (_testRunConfiguration.UnitTestProvider == TestProjectGenerator.UnitTestProvider.xUnit)
            {
                _vsTestExecutionDriver.Filter = $"Category={tag}";
            }
            else
            {
                _vsTestExecutionDriver.Filter = $"TestCategory={tag}";
            }

            ExecuteTests();
        }

        public void ExecuteTests()
        {
            PrepareTestExecution();

            _vsTestExecutionDriver.ExecuteTests();
        }

        private void PrepareTestExecution()
        {
            _solutionDriver.DefaultProject.Build();

            _solutionDriver.CompileSolution(BuildTool.MSBuild);
            _solutionDriver.CheckSolutionShouldHaveCompiled();
        }

        public async Task ExecuteTestsAsync()
        {
            PrepareTestExecution();

            await _vsTestExecutionDriver.ExecuteTestsAsync();
        }
    }
}
