using NUnit.Framework;

namespace TechTalk.SpecFlow.FeatureTests.ScopedSteps
{
    [Binding]
    public class GeneralBindings
    {
        private readonly StepTracker stepTracker;

        public GeneralBindings(StepTracker stepTracker)
        {
            this.stepTracker = stepTracker;
        }

        [When(@"I execute a scenario with a tag '([^']*)'")]
        public void WhenIExecuteAScenarioWithATag(string tag)
        {
            CollectionAssert.Contains(ScenarioContext.Current.ScenarioInfo.Tags ?? new string[0], tag, string.Format("The executed scenario is not tagged with tag '{0}'", tag));
        }

        [When(@"I execute a scenario in feature '([^']*)'")]
        public void WhenIExecuteAScenarioInFeature(string featureTitle)
        {
            Assert.AreEqual(featureTitle, FeatureContext.Current.FeatureInfo.Title, string.Format("The executed scenario is not in feature '{0}'", featureTitle));
        }

        [When(@"I execute a scenario in scenario '([^']*)'")]
        public void WhenIExecuteAScenarioInScenario(string scenarioTitle)
        {
            Assert.AreEqual(scenarioTitle, ScenarioContext.Current.ScenarioInfo.Title, string.Format("The executed scenario is not '{0}'", scenarioTitle));
        }

        [When(@"I execute a scenario with a tag '([^']*)' in feature '([^']*)'")]
        public void WhenIExecuteAScenarioWithATagInFeature(string tag, string featureTitle)
        {
            WhenIExecuteAScenarioWithATag(tag);
            WhenIExecuteAScenarioInFeature(featureTitle);
        }

        [Then(@"the step definition '(.*)' should be executed")]
        public void ThenTheStepDefinitionShouldBeExecuted(string stepTextPattern)
        {
            stepTracker.AssertStepExecuted(stepTextPattern);
        }

        [Then(@"the step definition '(.*)' should not be executed")]
        public void ThenTheStepDefinitionShouldNotBeExecuted(string stepTextPattern)
        {
            stepTracker.AssertStepNotExecuted(stepTextPattern);
        }

        [Then(@"the scenario should be executed successfully")]
        public void ThenTheScenarioShouldBeExecutedSuccessfully()
        {
            //nop
        }
    }
}