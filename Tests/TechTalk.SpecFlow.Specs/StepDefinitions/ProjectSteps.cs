using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ProjectSteps
    {
        private readonly ProjectsDriver _projectsDriver;
        private readonly SolutionDriver _solutionDriver;
        private readonly CompilationDriver _compilationDriver;

        public ProjectSteps(ProjectsDriver projectsDriver, SolutionDriver solutionDriver, CompilationDriver compilationDriver)
        {
            _projectsDriver = projectsDriver;
            _solutionDriver = solutionDriver;
            _compilationDriver = compilationDriver;
        }

        [Given(@"there is a SpecFlow project")]
        public void GivenThereIsASpecFlowProject()
        {
            _projectsDriver.CreateProject("C#");
        }

        [Given(@"it is using SpecFlow\.Tools\.MSBuild\.Generator")]
        public void GivenItIsUsingSpecFlow_Tools_MSBuild_Generator()
        {
        }


        [Given(@"parallel execution is enabled")]
        public void GivenParallelExecutionIsEnabled()
        {
            _projectsDriver.EnableTestParallelExecution();
        }


        [Given(@"I have a '(.*)' test project")]
        public void GivenIHaveATestProject(string language)
        {
            _projectsDriver.CreateProject(language);
        }

        [When(@"the project is compiled")]
        public void WhenTheProjectIsCompiled()
        {
            _compilationDriver.CompileSolution();
        }

        [Then(@"no compilation errors are reported")]
        public void ThenNoCompilationErrorsAreReported()
        {
            _compilationDriver.CheckSolutionShouldHaveCompiled();
        }

        [Then(@"is a compilation error")]
        public void ThenIsACompilationError()
        {
            _compilationDriver.CheckSolutionShouldHaveCompileError();
        }
    }
}
