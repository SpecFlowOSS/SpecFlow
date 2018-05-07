using System;
using FluentAssertions;
using SpecFlow.TestProjectGenerator.NewApi.Driver;
using SpecFlow.TestProjectGenerator.NewApi._3_NuGet;
using SpecFlow.TestProjectGenerator.NewApi._4_Compile;
using SpecFlow.TestProjectGenerator.NewApi._5_TestRun;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Assist;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExecutionSteps
    {
        private readonly SolutionDriver _solutionDriver;
        private readonly NuGet _nuGet;
        private readonly Compiler _compiler;
        private readonly VSTestExecutionDriver _vsTestExecution;
        private readonly ProjectsDriver _projectsDriver;
        private readonly NUnit3TestExecutionDriver nUnit3TestExecutionDriver;
        private readonly NUnit2TestExecutionDriver nUnit2TestExecutionDriver;
        private readonly XUnitTestExecutionDriver xUnitTestExecutionDriver;
        private readonly MsTestTestExecutionDriver msTestTestExecutionDriver;

        public ExecutionSteps(NUnit3TestExecutionDriver nUnit3TestExecutionDriver, NUnit2TestExecutionDriver nUnit2TestExecutionDriver, XUnitTestExecutionDriver xUnitTestExecutionDriver, MsTestTestExecutionDriver msTestTestExecutionDriver, SolutionDriver solutionDriver, NuGet nuGet, Compiler compiler, VSTestExecutionDriver vsTestExecution, ProjectsDriver projectsDriver)
        {
            this.nUnit3TestExecutionDriver = nUnit3TestExecutionDriver;
            this.nUnit2TestExecutionDriver = nUnit2TestExecutionDriver;
            this.xUnitTestExecutionDriver = xUnitTestExecutionDriver;
            _solutionDriver = solutionDriver;
            _nuGet = nuGet;
            _compiler = compiler;
            _vsTestExecution = vsTestExecution;
            _projectsDriver = projectsDriver;
            this.msTestTestExecutionDriver = msTestTestExecutionDriver;
        }

        [When(@"I execute the tests")]
        public void WhenIExecuteTheTests()
        {
            _solutionDriver.CompileSolution();
            _vsTestExecution.ExecuteTests();
        }

        [When(@"I execute the tests tagged with '@(.+)'")]
        public void WhenIExecuteTheTestsTaggedWithTag(string tag)
        {
            _solutionDriver.CompileSolution();
            _vsTestExecution.ExecuteTests(tag);
        }

        [When(@"I execute the tests with (.*)")]
        public void WhenIExecuteTheTestsWith(string unitTestProvider)
        {
            _solutionDriver.CompileSolution();
            _solutionDriver.CheckSolutionShouldHaveCompiled();

            // TODO: parse unit test provider before checking it
            switch (unitTestProvider)
            {
                case "xUnit":
                    _vsTestExecution.ExecuteTests();
                    break;
                case "NUnit.2":
                case "NUnit":
                case "MsTest":
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
