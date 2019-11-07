
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class MultipleSpecsProjectsSteps
    {
        private readonly ProjectsDriver _projectsDriver;
        private readonly SolutionDriver _solutionDriver;

        public MultipleSpecsProjectsSteps(ProjectsDriver projectsDriver, SolutionDriver solutionDriver)
        {
            _projectsDriver = projectsDriver;
            _solutionDriver = solutionDriver;
        }

        [Given("I have (Specs.Project.[A-Z]) and (Specs.Project.[A-Z]) using the same unit test provider")]
        public void GivenIHaveTwoSpecsProjectsWithTheSameUnitTestProvider(string projectName1, string projectName2)
        {
            _projectsDriver.CreateProject(projectName1, "C#");
            _projectsDriver.CreateProject(projectName2, "C#");
        }

        [Given(@"(Specs.Project.[A-Z]) references (Specs.Project.[A-Z])")]
        public void GivenSpecsProjectOneReferencesSpecsProjectTwo(string targetProjectName, string referencedProjectName)
        {
            _projectsDriver.AddProjectReference(referencedProjectName, targetProjectName);
        }

        [When(@"I build the solution using (\w+) with treat warnings as errors enabled")]
        public void WhenIBuildWithTreatWarningsAsErrorsEnabled(BuildTool buildTool)
        {
            _solutionDriver.CompileSolution(buildTool, true);
        }

        [Then(@"the build should succeed")]
        public void ThenTheBuildShouldSucceed()
        {
            _solutionDriver.CheckSolutionShouldHaveCompiled();
        }

    }
}
