using SpecFlow.TestProjectGenerator.NewApi.Driver;
using SpecFlow.TestProjectGenerator.NewApi._1_Memory;
using SpecFlow.TestProjectGenerator.NewApi._1_Memory.Extensions;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ExternalLibrarySteps : Steps
    {
        private readonly ProjectsDriver _projectsDriver;

        public ExternalLibrarySteps(ProjectsDriver projectsDriver)
        {
            _projectsDriver = projectsDriver;
        }

        [Given(@"there is an external class library project '(.*)'")]
        public void GivenThereIsAnExternalClassLibraryProject(string libraryName)
        {
            GivenThereIsAnExternalClassLibraryProject("C#", libraryName);
        }

        [Given(@"there is an external (.+) class library project '(.*)'")]
        public void GivenThereIsAnExternalClassLibraryProject(string language, string libraryName)
        {
            _projectsDriver.CreateProject(libraryName, language);
            _projectsDriver.Projects[libraryName].IsSpecFlowFeatureProject = false;
        }

        [Given(@"there is a reference between the SpecFlow project and the '(.*)' project")]
        public void GivenThereIsASpecFlowProjectWithAReferenceToTheExternalLibrary(string referencedProject)
        {
            _projectsDriver.AddProjectReference(referencedProject);
        }


    }
}
