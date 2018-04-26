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
        private readonly ProjectSteps projectSteps;
        private readonly SolutionDriver _solutionDriver;
        private readonly NuGet _nuGet;
        private readonly Compiler _compiler;
        private readonly VSTestExecutionDriver _vsTestExecution;
        private readonly ProjectDriver _projectDriver;
        private readonly AppConfigConfigurationDriver configurationDriver;
        private readonly NUnit3TestExecutionDriver nUnit3TestExecutionDriver;
        private readonly NUnit2TestExecutionDriver nUnit2TestExecutionDriver;
        private readonly XUnitTestExecutionDriver xUnitTestExecutionDriver;
        private readonly MsTestTestExecutionDriver msTestTestExecutionDriver;

        public ExecutionSteps(NUnit3TestExecutionDriver nUnit3TestExecutionDriver, NUnit2TestExecutionDriver nUnit2TestExecutionDriver, XUnitTestExecutionDriver xUnitTestExecutionDriver,
            AppConfigConfigurationDriver configurationDriver, MsTestTestExecutionDriver msTestTestExecutionDriver,
            ProjectSteps projectSteps, SolutionDriver solutionDriver, NuGet nuGet, Compiler compiler, VSTestExecutionDriver vsTestExecution, ProjectDriver projectDriver)
        {
            this.nUnit3TestExecutionDriver = nUnit3TestExecutionDriver;
            this.nUnit2TestExecutionDriver = nUnit2TestExecutionDriver;
            this.xUnitTestExecutionDriver = xUnitTestExecutionDriver;
            this.projectSteps = projectSteps;
            _solutionDriver = solutionDriver;
            _nuGet = nuGet;
            _compiler = compiler;
            _vsTestExecution = vsTestExecution;
            _projectDriver = projectDriver;
            this.msTestTestExecutionDriver = msTestTestExecutionDriver;
            this.configurationDriver = configurationDriver;
        }

        [When(@"I execute the tests")]
        public void WhenIExecuteTheTests()
        {
            _projectDriver.LastStuffBeforeWritingToDisk();
            _solutionDriver.WriteToDisk();

            _nuGet.Restore();

            var compileResult = _compiler.Run();

            if (!compileResult.Successful)
            {
                throw new Exception(compileResult.Output);
            }

            _vsTestExecution.ExecuteTests();
        }

        [When(@"I execute the tests tagged with '@(.+)'")]
        public void WhenIExecuteTheTestsTaggedWithTag(string tag)
        {
            nUnit3TestExecutionDriver.Include = tag;
            WhenIExecuteTheTests();
        }

        [When(@"I execute the tests with (.*)")]
        public void WhenIExecuteTheTestsWith(string unitTestProvider)
        {
            projectSteps.EnsureCompiled();

            switch (unitTestProvider)
            {
                case "NUnit.2":
                    nUnit2TestExecutionDriver.Execute();
                    break;
                case "NUnit":
                    nUnit3TestExecutionDriver.Execute();
                    break;
                case "MsTest":
                    msTestTestExecutionDriver.Execute();
                    break;
                case "xUnit":
                    xUnitTestExecutionDriver.Execute();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
