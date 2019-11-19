using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExecutionSteps
    {
        private readonly ExecutionDriver _executionDriver;
        private readonly CompilationDriver _compilationDriver;

        public ExecutionSteps(ExecutionDriver executionDriver, CompilationDriver compilationDriver)
        {
            _executionDriver = executionDriver;
            _compilationDriver = compilationDriver;
        }

        [When(@"I execute the tests")]
        public void WhenIExecuteTheTests()
        {
            _executionDriver.ExecuteTests();
        }

        [When(@"I execute the tests (once|twice|\d+ times)")]
        public void WhenIExecuteTheTestsTwice(uint times)
        {
            _executionDriver.ExecuteTestsTimes(times);
        }

        [When(@"I execute the tests tagged with '@(.+)'")]
        public void WhenIExecuteTheTestsTaggedWithTag(string tag)
        {
            _executionDriver.ExecuteTestsWithTag(tag);
        }

        [Given(@"MSBuild is used for compiling")]
        public void GivenMSBuildIsUsedForCompiling()
        {
            _compilationDriver.CompileSolution(BuildTool.MSBuild);
        }

        [Given(@"dotnet build is used for compiling")]
        public void GivenDotnetBuildIsUsedForCompiling()
        {
            _compilationDriver.CompileSolution(BuildTool.DotnetBuild);
        }

        [Given(@"dotnet msbuild is used for compiling")]
        public void GivenDotnetMsbuildIsUsedForCompiling()
        {
            _compilationDriver.CompileSolution(BuildTool.DotnetMSBuild);
        }

        [When(@"the solution is built (once|twice|\d+ times)")]
        public void WhenTheSolutionIsBuiltTwice(uint times)
        {
            _compilationDriver.CompileSolutionTimes(times);
        }
    }
}
