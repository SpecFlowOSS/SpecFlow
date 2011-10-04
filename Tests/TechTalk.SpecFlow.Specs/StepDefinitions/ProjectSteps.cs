using System.Linq;
using System.Text;
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
                var project = projectGenerator.GenerateProject(inputProjectDriver);
                projectCompiler.Compile(project);
                isCompiled = true;
            }
        }
    }
}
