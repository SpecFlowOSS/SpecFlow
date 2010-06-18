using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using NUnit.Framework;

namespace ReportingTest.SampleProject
{
    [Binding]
    public class StepDefinitions
    {
        [Given(@"I have a precondition that is (.*)")]
        [When(@"I do something that (.*)")]
        [Then(@"I have a postcondition that is (.*)")]
        public void GivenIHaveAPreconditionThatIs(string result)
        {
            switch(result.ToLower())
            {
                case "successful":
                case "works":
                    return;
                case "failing":
                    Assert.Fail("simulated failure");
                    break;
                case "pending":
                    ScenarioContext.Current.Pending();
                    break;
                default:
                    Assert.Fail("unknown result");
                    break;
            }
        }
    }
}
