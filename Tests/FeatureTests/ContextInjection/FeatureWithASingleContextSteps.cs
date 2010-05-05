using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace FeatureTests.ContextInjection
{
    [Binding]
    public class FeatureWithASingleContextSteps
    {
        private readonly SingleContext _context;

        public FeatureWithASingleContextSteps(SingleContext context)
        {
            _context = context;
            _context.WasCreatedBy = "Feature With A Single Context";
        }

        [Given("a feature which requires a single context")]
        public void GivenAFeatureWhichRequiresASingleContext()
        {
        }

        [Then("the context is set")]
        public void ThenTheContextIsSet()
        {
            Assert.That(_context, Is.Not.Null);
        }
    }
}
