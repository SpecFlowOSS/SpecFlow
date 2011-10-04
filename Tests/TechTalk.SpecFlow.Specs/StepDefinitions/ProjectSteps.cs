using System;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Specs.Drivers.MsBuild;
using Should;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ProjectSteps
    {
        private readonly InputProjectDriver inputProjectDriver;
        private readonly ProjectGenerator projectGenerator;
        private readonly ProjectCompiler projectCompiler;

        public ProjectSteps(InputProjectDriver inputProjectDriver, ProjectGenerator projectGenerator, ProjectCompiler projectCompiler)
        {
            this.inputProjectDriver = inputProjectDriver;
            this.projectCompiler = projectCompiler;
            this.projectGenerator = projectGenerator;
        }

        [Given(@"there is a SpecFlow project")]
        public void GivenThereIsASpecFlowProject()
        {
            GivenThereIsASpecFlowProject("SpecFlow.TestProject");
        }

        [Given(@"there is a SpecFlow project '(.*)'")]
        public void GivenThereIsASpecFlowProject(string projectName)
        {
            inputProjectDriver.ProjectName = projectName;
        }

        private bool isCompiled = false;
        private Exception CompilationError;

//        [BeforeScenarioBlock]
//        public void CompileProject()
//        {
//            if ((ScenarioContext.Current.CurrentScenarioBlock == ScenarioBlock.When))
//            {
//                EnsureCompiled();
//            }
//        }
//
        public void EnsureCompiled()
        {
            if (!isCompiled)
            {
                try
                {
                    CompileInternal();
                }
                finally
                {
                    isCompiled = true;
                }
            }
        }

        private void CompileInternal()
        {
            var project = projectGenerator.GenerateProject(inputProjectDriver);
            projectCompiler.Compile(project);
        }

        [When(@"the project is compiled")]
        public void WhenTheProjectIsCompiled()
        {
            try
            {
                CompilationError = null;
                CompileInternal();
            }
            catch (Exception ex)
            {
                CompilationError = ex;
            }
        }

        [Then(@"no compilation errors are reported")]
        public void ThenNoCompilationErrorsAreReported()
        {
            CompilationError.ShouldBeNull();
        }
    }
}
