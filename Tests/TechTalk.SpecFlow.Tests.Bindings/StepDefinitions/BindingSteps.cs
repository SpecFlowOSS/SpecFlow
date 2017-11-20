using TechTalk.SpecFlow.Tests.Bindings.Drivers;

namespace TechTalk.SpecFlow.Tests.Bindings.StepDefinitions
{
    [Binding]
    public class BindingSteps
    {
        private readonly InputProjectDriver inputProjectDriver;
        private readonly HooksDriver hooksDriver;

        public BindingSteps(InputProjectDriver inputProjectDriver, HooksDriver hooksDriver)
        {
            this.inputProjectDriver = inputProjectDriver;
            this.hooksDriver = hooksDriver;
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
            inputProjectDriver.AddStepBinding(scenarioBlock, ".*", "throw new System.Exception(\"simulated failure\");", "Throw new System.Exception(\"simulated failure\")");
        }

        [Given(@"all '(.*)' steps are bound and pass")]
        public void GivenAllStepsAreBoundAndPass(ScenarioBlock scenarioBlock)
        {
            inputProjectDriver.AddStepBinding(scenarioBlock, ".*", "//pass", "'pass");
        }

        [Given(@"the following hooks?")]
        [Given(@"the following step definitions?")]
        [Given(@"the following step argument transformations?")]
        [Given(@"the following binding methods?")]
        public void GivenTheFollowingBindings(string bindingCode)
        {
            inputProjectDriver.AddBindingCode(bindingCode);
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
            inputProjectDriver.AddRawBindingClass(rawBindingClass);
        }
    }
}