using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class ExecutionDriver
    {
        private readonly VSTestExecutionDriver _vsTestExecutionDriver;
        private readonly CompilationDriver _compilationDriver;
        private readonly TestRunConfiguration _testRunConfiguration;

        private List<string> _errorMessages;

        public ExecutionDriver(VSTestExecutionDriver vsTestExecutionDriver, CompilationDriver compilationDriver, TestRunConfiguration testRunConfiguration)
        {
            _vsTestExecutionDriver = vsTestExecutionDriver;
            _compilationDriver = compilationDriver;
            _testRunConfiguration = testRunConfiguration;
            _errorMessages = new List<string>();
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
                var testExecutionResult = _vsTestExecutionDriver.ExecuteTests();
                if (testExecutionResult.Failed > 0)
                {
                    _errorMessages.AddRange(testExecutionResult.TestResults.Where(tr => !string.IsNullOrEmpty(tr.ErrorMessage)).Select(testResult => testResult.ErrorMessage));
                }
            }

            _errorMessages.Should().BeEmpty();
        }
    }
}
