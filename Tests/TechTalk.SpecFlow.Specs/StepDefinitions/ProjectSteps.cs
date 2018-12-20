﻿using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ProjectSteps
    {
        private readonly ProjectsDriver _projectsDriver;
        private readonly SolutionDriver _solutionDriver;

        public ProjectSteps(ProjectsDriver projectsDriver, SolutionDriver solutionDriver)
        {
            _projectsDriver = projectsDriver;
            _solutionDriver = solutionDriver;
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
            _solutionDriver.CompileSolution(BuildTool.MSBuild);
        }

        [Then(@"no compilation errors are reported")]
        public void ThenNoCompilationErrorsAreReported()
        {
            _solutionDriver.CheckSolutionShouldHaveCompiled();
        }

        [Then(@"is a compilation error")]
        public void ThenIsACompilationError()
        {
            _solutionDriver.CheckSolutionShouldHaseCompileError();
        }

    }
}
