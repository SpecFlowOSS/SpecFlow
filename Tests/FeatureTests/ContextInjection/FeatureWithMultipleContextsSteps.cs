using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace FeatureTests.ContextInjection
{
    [Binding]
    public class FeatureWithMultipleContextsSteps
    {
        private readonly SingleContext _context1;
        private readonly SingleContext _context2;

        public FeatureWithMultipleContextsSteps(SingleContext context1, SingleContext context2)
        {
            _context1 = context1;
            _context2 = context2;
        }

        [Given("a feature which requires multiple contexts")]
        public void GivenAFeatureWhichRequiresMultipleContexts()
        {
        }

        [Then("the contexts are set")]
        public void ThenTheContextsAreSet()
        {
            Assert.That(_context1, Is.Not.Null);
            Assert.That(_context2, Is.Not.Null);
        }
    }
}
