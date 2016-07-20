using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class IntValueRetrieverTests
    {
        [Test]
        public void Returns_an_integer_when_passed_an_integer_value()
        {
            var retriever = new IntValueRetriever();
            retriever.GetValue("1").Should().Be(1);
            retriever.GetValue("3").Should().Be(3);
            retriever.GetValue("30").Should().Be(30);
            retriever.GetValue("1234567890").Should().Be(1234567890);
        }

        [Test]
        public void Returns_negative_numbers_when_passed_a_negative_value()
        {
            var retriever = new IntValueRetriever();
            retriever.GetValue("-1").Should().Be(-1);
            retriever.GetValue("-5").Should().Be(-5);
        }

        [Test]
        public void Returns_a_zero_when_passed_an_invalid_int()
        {
            var retriever = new IntValueRetriever();
            retriever.GetValue("x").Should().Be(0);
            retriever.GetValue("").Should().Be(0);
            retriever.GetValue("every good boy does fine").Should().Be(0);
        }
    }
}