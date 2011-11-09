using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class BindingSteps
    {
        private readonly InputProjectDriver inputProjectDriver;

        public BindingSteps(InputProjectDriver inputProjectDriver)
        {
            this.inputProjectDriver = inputProjectDriver;
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
            inputProjectDriver.AddStepBinding(scenarioBlock, ".*", code: "throw new System.Exception(\"simulated failure\");");
        }

        [Given(@"all '(.*)' steps are bound and pass")]
        public void GivenAllStepsAreBoundAndPass(ScenarioBlock scenarioBlock)
        {
            inputProjectDriver.AddStepBinding(scenarioBlock, ".*", code: "//pass");
        }

        [Given(@"the following hooks?")]
        [Given(@"the following step definitions?")]
        [Given(@"the following step argument transformations?")]
        [Given(@"the following binding methods?")]
        public void GivenTheFollowingBindings(string bindingCode)
        {
            inputProjectDriver.AddBindingCode(bindingCode);
        }

        [Given(@"a hook '(.*)' for '(.*)'")]
        public void GivenAnEventBindingFor(string methodName, string eventType)
        {
            inputProjectDriver.AddEventBinding(eventType, code: "//pass", methodName: methodName);
        }

        [Given(@"the following binding class")]
        [Given(@"the following class")]
        public void GivenTheFollowingBindingClass(string rawBindingClass)
        {
            inputProjectDriver.AddRawBindingClass(rawBindingClass);
        }
    }
}
