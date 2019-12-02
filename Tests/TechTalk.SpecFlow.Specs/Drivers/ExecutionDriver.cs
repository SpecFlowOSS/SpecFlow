using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class ExecutionDriver
    {
        private readonly VSTestExecutionDriver _vsTestExecutionDriver;
        private readonly CompilationDriver _compilationDriver;
        private readonly TestRunConfiguration _testRunConfiguration;

        public ExecutionDriver(VSTestExecutionDriver vsTestExecutionDriver, CompilationDriver compilationDriver, TestRunConfiguration testRunConfiguration)
        {
            _vsTestExecutionDriver = vsTestExecutionDriver;
            _compilationDriver = compilationDriver;
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
            ExecuteTestsTimes(1);
        }

        public void ExecuteTestsTimes(uint times)
        {
            if (!_compilationDriver.HasTriedToCompile)
            {
                _compilationDriver.CompileSolution();
            }

            for (uint currentTime = 0; currentTime < times; currentTime++)
            {
                _vsTestExecutionDriver.ExecuteTests();
            }
        }
    }
}
