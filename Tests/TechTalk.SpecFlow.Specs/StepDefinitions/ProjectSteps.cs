using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using TechTalk.SpecFlow.Specs.Drivers;
using TechTalk.SpecFlow.Specs.Drivers.MsBuild;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class ProjectSteps
    {
        private readonly InputProjectDriver inputProjectDriver;
        private readonly ProjectGenerator projectGenerator;
        private readonly ProjectCompiler projectCompiler;
        private readonly HooksDriver _hooksDriver;

        public ProjectSteps(InputProjectDriver inputProjectDriver, ProjectGenerator projectGenerator, ProjectCompiler projectCompiler, HooksDriver hooksDriver)
        {
            this.inputProjectDriver = inputProjectDriver;
            this.projectCompiler = projectCompiler;
            _hooksDriver = hooksDriver;
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

        [Given(@"I have a '(.*)' test project")]
        public void GivenIHaveATestProject(string language)
        {
            inputProjectDriver.Language = language;
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

            _hooksDriver.EnsureInitialized();
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
            CompilationError.Should().BeNull();
        }
    }
}
