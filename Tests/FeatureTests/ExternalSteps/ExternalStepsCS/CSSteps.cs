using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace ExternalStepsCS
{
    [Binding]
    public class CSSteps
    {
        [Given(@"I have external step definitions in a separate assembly referenced by this project")]
        public void GivenIHaveExternalStepDefinitionsInASeparateAssemblyReferencedByThisProject()
        {
            ScenarioContext.Current["counter"] = 1;
        }

        [When(@"I call those steps")]
        public void WhenICallThoseSteps()
        {
            ScenarioContext.Current["counter"] = (int)ScenarioContext.Current["counter"] + 1;
        }

        [Then("the scenario should pass")]
        public void GivenAFeatureWhichRequiresADependentContext()
        {
            Assert.AreEqual(2,(int)ScenarioContext.Current["counter"]);
        }
    }
}
