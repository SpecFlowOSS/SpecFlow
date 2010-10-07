namespace TechTalk.SpecFlow.FeatureTests.ScopedSteps
{
    using System;
    using NUnit.Framework;

    [Binding("Addition")]
    public class FeatureScopeImplementationScoped
    {
        [Given(@"I have entered 50 into the calculator")]
        public void GivenIHaveEntered50IntoTheCalculator()
        {
            Console.WriteLine("SCOPED");
        }

        [Given(@"I have entered 70 into the calculator")]
        public void GivenIHaveEntered70IntoTheCalculator()
        {
            Console.WriteLine("SCOPED");
        }

        [When(@"I press add")]
        public void WhenIPressAdd()
        {
            Console.WriteLine("SCOPED");
        }
        [Then(@"the result should be (.*) on the screen")]
        public void ThenTheResultShouldBe120OnTheScreen(string value)
        {
            Console.WriteLine("SCOPED");
            Assert.AreEqual("120", value, "Wrong value in parameter");
        }
    }

    [Binding]
    public class FeatureScopeImplementationUnScoped
    {
        [Given(@"I have entered 45 into the calculator")]
        public void GivenIHaveEntered45IntoTheCalculator()
        {
            Console.WriteLine("Non Scoped");
        }

        [Given(@"I have entered 55 into the calculator")]
        public void GivenIHaveEntered55IntoTheCalculator()
        {
            Console.WriteLine("Non Scoped");
        }

        [When(@"I press add")]
        public void WhenIPressAdd()
        {
            Console.WriteLine("Non Scoped");
        }
        [Then(@"the result should be (.*) on the screen")]
        public void ThenTheResultShouldBe120OnTheScreen(string value)
        {
            Console.WriteLine("Non Scoped");
            Assert.AreEqual("100", value, "Wrong value in parameter");
        }
    }
}