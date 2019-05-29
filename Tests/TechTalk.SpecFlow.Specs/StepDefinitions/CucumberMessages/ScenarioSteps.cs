using System;
using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class ScenarioSteps
    {
        private readonly TestSuiteSetupDriver _testSuiteSetupDriver;

        public ScenarioSteps(TestSuiteSetupDriver testSuiteSetupDriver)
        {
            _testSuiteSetupDriver = testSuiteSetupDriver;
        }

        [Given(@"there are (\d+) scenarios")]
        public void GivenThereAreScenarios(int scenarios)
        {
            _testSuiteSetupDriver.AddScenarios(scenarios);
        }

        [Given(@"there is a scenario")]
        public void GivenThereIsAScenario()
        {
            _testSuiteSetupDriver.AddScenarios(1);
        }

        [Given(@"there is a scenario with PickleId '(.*)'")]
        public void GivenThereIsAScenarioWithPickleId(Guid pickleId)
        {
            _testSuiteSetupDriver.AddScenario(pickleId);
        }
    }
}
