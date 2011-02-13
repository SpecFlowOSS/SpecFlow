using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TechTalk.SpecFlow.FeatureTests.TaggedExamples
{
    [Binding]
    public class TaggedExamplesBindings
    {
        [Given(@"I have a templated step for (.*)")]
        public void GivenIHaveATemplatedStepForVariant(string variant)
        {
            //nop
        }

        [When(@"I tag the examples with tag (.*)")]
        public void WhenITagTheExamplesWithTag(string tag)
        {
            //nop
        }

        [Then(@"the execution should be scoped with tag (.*)")]
        public void ThenTheExecutionShouldBeScopedWithTag(string tag)
        {
            if (!string.IsNullOrEmpty(tag))
                CollectionAssert.Contains(ScenarioContext.Current.ScenarioInfo.Tags, tag);
        }
    }
}
