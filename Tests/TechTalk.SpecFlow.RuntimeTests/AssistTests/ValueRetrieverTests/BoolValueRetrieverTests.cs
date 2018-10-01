using FluentAssertions;
using NUnit.Framework;
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
            retriever.GetValue("True").Should().BeTrue();
        }

        [Test]
        public void Returns_false_when_the_value_is_False()
        {
            var retriever = new BoolValueRetriever();
            retriever.GetValue("False").Should().BeFalse();
        }

        [Test]
        public void Returns_true_when_the_value_is_true()
        {
            var retriever = new BoolValueRetriever();
            retriever.GetValue("true").Should().BeTrue();
        }

        [Test]
        public void Returns_true_when_the_value_is_1()
        {
            var retriever = new BoolValueRetriever();
            retriever.GetValue("1").Should().BeTrue();
        }

        [Test]
        public void Returns_false_when_the_value_is_0()
        {
            var retriever = new BoolValueRetriever();
            retriever.GetValue("0").Should().BeFalse();
        }

        [Test]
        public void Returns_false_for_data_that_is_not_bool()
        {
            var retriever = new BoolValueRetriever();
            retriever.GetValue("sssssdfsd").Should().BeFalse();
            retriever.GetValue(null).Should().BeFalse();
            retriever.GetValue("").Should().BeFalse();
            retriever.GetValue("this is false").Should().BeFalse();
        }
    }
}