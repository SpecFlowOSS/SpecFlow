using System;
using TechTalk.SpecFlow.TestProjectGenerator.CucumberMessages;

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

        [Given(@"there are '(\d+)' scenarios")]
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

        [Given(@"there are two step definitions with identical bindings")]
        public void GivenThereAreTwoStepDefinitionsWithIdenticalRegex()
        {
            _testSuiteSetupDriver.AddDuplicateStepDefinition();
        }

        [Given(@"there are no matching step definitions")]
        public void GivenThereAreNoMatchingStepDefinitions()
        {
            _testSuiteSetupDriver.AddNotMatchingStepDefinition();
        }

        [Given(@"there is a scenario with the following steps: '(.*)'")]
        public void GivenThereIsAScenarioWithTheFollowingSteps(string step)
        {
            _testSuiteSetupDriver.AddScenarioWithGivenStep(step);
        }

        [Given(@"with step definitions in the following order: '(.*)'")]
        public void GivenWithStepDefinitionsInTheFollowingOrder(string stepDefinitionOrder)
        {
            _testSuiteSetupDriver.AddStepDefinitionsFromStringList(stepDefinitionOrder);
        }
    }
}
