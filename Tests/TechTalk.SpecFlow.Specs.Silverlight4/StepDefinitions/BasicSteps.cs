using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace TechTalk.SpecFlow.Specs.Silverlight4.StepDefinitions
{
    [Binding]
    public class BasicSteps
    {
        [Given(@"I can do something")]
        public void GivenICanDoSomething()
        {
            ScenarioContext.Current["value"] = "something";
        }

        [Then(@"something has occurred")]
        public void ThenSomethingHasOccurred()
        {
            Assert.AreEqual((string)ScenarioContext.Current["value"], "something");
        }
    }
}
