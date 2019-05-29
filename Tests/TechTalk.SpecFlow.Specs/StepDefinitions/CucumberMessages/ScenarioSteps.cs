using System;

namespace TechTalk.SpecFlow.Specs.StepDefinitions.CucumberMessages
{
    [Binding]
    public class ScenarioSteps
    {
        [Given(@"there are (\d+) scenarios")]
        public void GivenThereAreScenarios(int scenarios)
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"there is a scenario")]
        public void GivenThereIsAScenario()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"there is a scenario with PickleId '(.*)'")]
        public void GivenThereIsAScenarioWithPickleId(Guid pickleId)
        {
            ScenarioContext.Current.Pending();
        }

    }
}
