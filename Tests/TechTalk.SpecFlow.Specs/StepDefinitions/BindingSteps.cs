using System.IO;
using SpecFlow.TestProjectGenerator.NewApi.Driver;
using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class BindingSteps
    {
        private readonly InputProjectDriver inputProjectDriver;
        private readonly HooksDriver hooksDriver;
        private readonly ProjectsDriver _projectsDriver;

        public BindingSteps(InputProjectDriver inputProjectDriver, HooksDriver hooksDriver, ProjectsDriver projectsDriver)
        {
            this.inputProjectDriver = inputProjectDriver;
            this.hooksDriver = hooksDriver;
            _projectsDriver = projectsDriver;
        }

        [Given(@"all steps are bound and pass")]
        public void GivenAllStepsAreBoundAndPass()
        {
            GivenAllStepsAreBoundAndPass(ScenarioBlock.Given);
            GivenAllStepsAreBoundAndPass(ScenarioBlock.When);
            GivenAllStepsAreBoundAndPass(ScenarioBlock.Then);
        }

        [Given(@"all steps are bound and fail")]
        public void GivenAllStepsAreBoundAndFail()
        {
            GivenAllStepsAreBoundAndFail(ScenarioBlock.Given);
            GivenAllStepsAreBoundAndFail(ScenarioBlock.When);
            GivenAllStepsAreBoundAndFail(ScenarioBlock.Then);
        }

        [Given(@"all '(.*)' steps are bound and fail")]
        public void GivenAllStepsAreBoundAndFail(ScenarioBlock scenarioBlock)
        {
            _projectsDriver.AddStepBinding(scenarioBlock.ToString(), ".*", "throw new System.Exception(\"simulated failure\");", "Throw New System.Exception(\"simulated failure\")");
        }

        [Given(@"all '(.*)' steps are bound and pass")]
        public void GivenAllStepsAreBoundAndPass(ScenarioBlock scenarioBlock)
        {
            _projectsDriver.AddStepBinding(scenarioBlock.ToString(), ".*", "//pass", "'pass");
        }

        [Given(@"the following step definition in the project '(.*)'")]
        [Given(@"the following binding class in the project '(.*)'")]
        public void GivenTheFollowingStepDefinitionInTheProject(string projectName, string stepDefinition)
        {
            _projectsDriver.AddStepBinding(projectName, stepDefinition);
        }

        [Given(@"the following hooks?")]
        [Given(@"the following step definitions?")]
        [Given(@"the following step argument transformations?")]
        [Given(@"the following binding methods?")]
        public void GivenTheFollowingBindings(string bindingCode)
        {
            _projectsDriver.AddStepBinding(bindingCode);
        }

        [Given(@"a hook '(.*)' for '([^']*)'")]
        public void GivenAnEventBindingFor(string methodName, string eventType)
        {
            inputProjectDriver.AddEventBinding(eventType, hooksDriver.GetHookLogStatement(methodName), methodName);
        }

        [Given(@"a hook '(.*)' for '([^']*)' with order '([^']*)'")]
        public void GivenAHookForWithOrder(string methodName, string eventType, int hookOrder)
        {
            inputProjectDriver.AddEventBinding(eventType, hooksDriver.GetHookLogStatement(methodName), methodName, hookOrder);
        }

        [Given(@"the following binding class")]
        [Given(@"the following class")]
        public void GivenTheFollowingBindingClass(string rawBindingClass)
        {
            _projectsDriver.AddBindingClass(rawBindingClass);
        }
    }
}