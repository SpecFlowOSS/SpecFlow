using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class TimespanValueRetrieverTests
    {
        [Test]
        public void Returns_an_integer_when_passed_an_integer_value()
        {
            var retriever = new TimeSpanValueRetriever();
            retriever.GetValue("20:00:00").Should().Be(System.TimeSpan.Parse("20:00:00"));
            retriever.GetValue("20:00:01").Should().Be(System.TimeSpan.Parse("20:00:01"));
            retriever.GetValue("00:00:00").Should().Be(System.TimeSpan.Parse("00:00:00"));
        }

        [Test]
        public void It_handles_timespans()
        {
            var retriever = new TimeSpanValueRetriever();
            var empty = new System.Collections.Generic.KeyValuePair<string, string>();
            retriever.CanRetrieve(empty, typeof(System.TimeSpan)).Should().BeTrue();
            retriever.CanRetrieve(empty, typeof(System.String)).Should().BeFalse();
            retriever.CanRetrieve(empty, typeof(System.Boolean)).Should().BeFalse();
        }
    }
}