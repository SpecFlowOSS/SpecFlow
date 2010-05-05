using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace FeatureTests.ContextInjection
{
    [Binding]
    public class FeatureWithADependentContextSteps
    {
        private readonly SingleContext _context;

        public FeatureWithADependentContextSteps(SingleContext context)
        {
            _context = context;
        }

        [Given("a feature which requires a dependent context")]
        public void GivenAFeatureWhichRequiresADependentContext()
        {
        }

        [Then("the context was created by the feature with a single context scenario")]
        public void ThenTheContextWasCreatedByTheFeatureWithASingleContextScenario()
        {
            Assert.That(_context.WasCreatedBy, Is.EqualTo("Feature With A Single Context")); 
        }
    }
}
