using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExecutionSteps
    {
        private readonly SolutionDriver _solutionDriver;
        private readonly ExecutionDriver _executionDriver;

        public ExecutionSteps(SolutionDriver solutionDriver, ExecutionDriver executionDriver)
        {
            _solutionDriver = solutionDriver;
            _executionDriver = executionDriver;
        }

        [When(@"I execute the tests")]
        public void WhenIExecuteTheTests()
        {
            _executionDriver.ExecuteTests();
        }

        [When(@"I execute the tests tagged with '@(.+)'")]
        public void WhenIExecuteTheTestsTaggedWithTag(string tag)
        {
            _executionDriver.ExecuteTestsWithTag(tag);
        }

        [Given(@"MSBuild is used for compiling")]
        public void GivenMSBuildIsUsedForCompiling()
        {
            _solutionDriver.CompileSolution(BuildTool.MSBuild);
        }

        [Given(@"dotnet build is used for compiling")]
        public void GivenDotnetBuildIsUsedForCompiling()
        {
            _solutionDriver.CompileSolution(BuildTool.DotnetBuild);
        }

        [Given(@"dotnet msbuild is used for compiling")]
        public void GivenDotnetMsbuildIsUsedForCompiling()
        {
            _solutionDriver.CompileSolution(BuildTool.DotnetMSBuild);
        }


    }
}
