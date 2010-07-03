using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace $rootnamespace$
{
    [Binding]
    public class $safeitemname$
    {
        [BeforeStep]
        public void BeforeStep()
        {
            //TODO: implement logic that has to run before each scenario step
            // For storing and retrieving scenario-specific data, 
            // the instance fields of the class or the
            //     ScenarioContext.Current
            // collection can be used.
            // For storing and retrieving feature-specific data, the 
            //     FeatureContext.Current
            // collection can be used.
            // Use the attribute overload to specify tags. If tags are specified, the event 
            // handler will be executed only if any of the tags are specified for the 
            // feature or the scenario.
            //     [BeforeStep("mytag")]
        }

        [AfterStep]
        public void AfterStep()
        {
            //TODO: implement logic that has to run after each scenario step
        }

        [BeforeScenarioBlock]
        public void BeforeScenarioBlock()
        {
            //TODO: implement logic that has to run before each scenario block (given-when-then)
        }

        [AfterScenarioBlock]
        public void AfterScenarioBlock()
        {
            //TODO: implement logic that has to run after each scenario block (given-when-then)
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            //TODO: implement logic that has to run before executing each scenario
        }

        [AfterScenario]
        public void AfterScenario()
        {
            //TODO: implement logic that has to run after executing each scenario
        }

        [BeforeFeature]
        public static void BeforeFeature()
        {
            //TODO: implement logic that has to run before executing each feature
        }

        [AfterFeature]
        public static void AfterFeature()
        {
            //TODO: implement logic that has to run after executing each feature
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            //TODO: implement logic that has to run before the entire test run
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            //TODO: implement logic that has to run after the entire test run
        }
    }
}
