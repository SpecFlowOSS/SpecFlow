using System;
using NUnit.Framework;

namespace TechTalk.SpecFlow.FeatureTests.BeforeAfterHooks
{
    [Binding]
    public class BeforeAfterHooksSteps
    {
        private static bool _beforeTestRunHookExecuted;
        private static bool _beforeFeatureHookExecuted;
        private bool _beforeScenarioHookExecuted;
        private bool _beforeScenarioBlockHookExecuted;
        private bool _beforeStepHookExecuted;
        private static bool _afterFeatureHookExecuted;
        private static bool _afterScenarioHookExecuted;
        private static bool _afterScenarioBlockHookExecuted;
        private static bool _afterStepHookExecuted;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            _beforeTestRunHookExecuted = true;
        }

        [BeforeFeature]
        public static void BeforeFeature()
        {
            _beforeFeatureHookExecuted = true;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _beforeScenarioHookExecuted = true;
        }

        [BeforeScenarioBlock]
        public void BeforeScenarioBlock()
        {
            _beforeScenarioBlockHookExecuted = true;
        }

        [BeforeStep]
        public void BeforeStep()
        {
            _beforeStepHookExecuted = true;
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            // Testing AfterTestRun is tricky as it would probably involve manipulating the AppDomain
            // Its been manually tested and verified that this code block is hit, but if something changes
            // which stops it from being called, we'll never know...

            Assert.That(_afterFeatureHookExecuted, Is.True);
            Assert.That(_afterScenarioHookExecuted, Is.True);
            Assert.That(_afterScenarioBlockHookExecuted, Is.True);
            Assert.That(_afterStepHookExecuted, Is.True);
        }

        [AfterFeature]
        public static void AfterFeature()
        {
            _afterFeatureHookExecuted = true;
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _afterScenarioHookExecuted = true;
        }

        [AfterScenarioBlock]
        public void AfterScenarioBlock()
        {
            _afterScenarioBlockHookExecuted = true;
        }

        [AfterStep]
        public void AfterStep()
        {
            _afterStepHookExecuted = true;
        }

        [Given(@"the scenario is running")]
        public void GivenTheScenarioIsRunning()
        {
        }

        [Then(@"the BeforeTestRun hook should have been executed")]
        public void ThenTheBeforeTestRunHookShouldHaveBeenExecuted()
        {
            Assert.That(_beforeTestRunHookExecuted, Is.True);
        }

        [Then(@"the BeforeFeature hook should have been executed")]
        public void ThenTheBeforeFeatureHookShouldHaveBeenExecuted()
        {
            Assert.That(_beforeFeatureHookExecuted, Is.True);
        }

        [Then(@"the BeforeScenario hook should have been executed")]
        public void ThenTheBeforeScenarioHookShouldHaveBeenExecuted()
        {
            Assert.That(_beforeScenarioHookExecuted, Is.True);
        }

        [Then(@"the BeforeScenarioBlock hook should have been executed")]
        public void ThenTheBeforeScenarioBlockHookShouldHaveBeenExecuted()
        {
            Assert.That(_beforeScenarioBlockHookExecuted, Is.True);
        }

        [Then(@"the BeforeStep hook should have been executed")]
        public void ThenTheBeforeStepHookShouldHaveBeenExecuted()
        {
            Assert.That(_beforeStepHookExecuted, Is.True);
        }
    }
}