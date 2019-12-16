using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ProjectSteps
    {
        private readonly ProjectsDriver _projectsDriver;
        private readonly CompilationDriver _compilationDriver;
        private readonly CompilationResultDriver _compilationResultDriver;

        public ProjectSteps(ProjectsDriver projectsDriver, CompilationDriver compilationDriver, CompilationResultDriver compilationResultDriver)
        {
            _projectsDriver = projectsDriver;
            _compilationDriver = compilationDriver;
            _compilationResultDriver = compilationResultDriver;
        }

        [Given(@"there is a SpecFlow project")]
        public void GivenThereIsASpecFlowProject()
        {
            _projectsDriver.CreateSpecFlowProject("C#");
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

        [When(@"I compile the solution")]
        public void WhenTheProjectIsCompiled()
        {
            _compilationDriver.CompileSolution();
        }

        [Then(@"no compilation errors are reported")]
        public void ThenNoCompilationErrorsAreReported()
        {
            _compilationResultDriver.CheckSolutionShouldHaveCompiled();
        }

        [Then(@"is a compilation error")]
        public void ThenIsACompilationError()
        {
            _compilationResultDriver.CheckSolutionShouldHaveCompileError();
        }
    }
}
