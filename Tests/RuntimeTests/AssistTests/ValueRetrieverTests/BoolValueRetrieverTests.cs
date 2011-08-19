using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class BoolValueRetrieverTests
    {
        [Test]
        public void Returns_true_when_the_value_is_True()
        {
            var retriever = new BoolValueRetriever();
            retriever.GetValue("True").ShouldBeTrue();
        }

        [Test]
        public void Returns_false_when_the_value_is_False()
        {
            var retriever = new BoolValueRetriever();
            retriever.GetValue("False").ShouldBeFalse();
        }

        [Test]
        public void Returns_true_when_the_value_is_true()
        {
            var retriever = new BoolValueRetriever();
            retriever.GetValue("true").ShouldBeTrue();
        }

        [Test]
        public void Returns_false_for_data_that_is_not_bool()
        {
            var retriever = new BoolValueRetriever();
            retriever.GetValue("sssssdfsd").ShouldBeFalse();
            retriever.GetValue(null).ShouldBeFalse();
            retriever.GetValue("").ShouldBeFalse();
            retriever.GetValue("this is false").ShouldBeFalse();
        }
    }
}