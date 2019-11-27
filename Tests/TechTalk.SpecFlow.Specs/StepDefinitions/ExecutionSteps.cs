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

        [Given(@"'(dotnet msbuild|dotnet build|MSBuild)' is used for compiling")]
        public void GivenIsUsedForCompiling(BuildTool buildTool)
        {
            _compilationDriver.SetBuildTool(buildTool);
        }

        [When(@"I build the solution using '(dotnet msbuild|dotnet build|MSBuild)'")]
        [When(@"I compile the solution using '(dotnet msbuild|dotnet build|MSBuild)'")]
        public void WhenIBuildTheSolutionUsing(BuildTool buildTool)
        {
            _compilationDriver.CompileSolution(buildTool);
        }

        [When(@"the solution is built (once|twice|\d+ times)")]
        public void WhenTheSolutionIsBuiltTwice(uint times)
        {
            _compilationDriver.CompileSolutionTimes(times);
        }
    }
}
