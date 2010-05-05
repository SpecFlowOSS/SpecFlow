using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace FeatureTests.ContextInjection
{
    [Binding]
    public class FeatureWithNoContextSteps
    {
        [Given("a feature which requires no context")]
        public void GivenAFeatureWhichRequiresNoContext()
        {
        }

        [Then("everything is dandy")]
        public void ThenEverythingIsDandy()
        {
        }
    }
}
