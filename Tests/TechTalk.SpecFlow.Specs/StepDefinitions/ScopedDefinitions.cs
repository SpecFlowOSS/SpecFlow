using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace UnitTestProject1
{
    [Binding, Scope(Tag = "featureTag")]
    public sealed class StepDefinition1
    {

        [Given("I have entered (.*) into the calculator")]
        public void GivenIHaveEnteredSomethingIntoTheCalculator(int number)
        {
            Debug.WriteLine("Given I have entered " + number + " into the calculator");
        }

        [When(@"I press add"), Scope(Tag = "scenarioTag1")]
        public void WhenIPressAdd()
        {
            Debug.WriteLine("When I press Add ");
        }

        [When(@"I press add"), Scope(Tag = "scenarioTag2")]
        public void WhenIPressAddVariant()
        {
            Debug.WriteLine("When I press Add Variant");
        }


        [Then("the result should be (.*) on the screen")]
        public void ThenTheResultShouldBe(int result)
        {
            //TODO: implement assert (verification) logic

            Debug.WriteLine("Then the result should be " + result + " on the screen");
        }
    }
}
