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
        [Then("the scenario should pass")]
        public void GivenAFeatureWhichRequiresADependentContext()
        {
            Assert.AreEqual(2,(int)ScenarioContext.Current["counter"]);
        }
    }
}
