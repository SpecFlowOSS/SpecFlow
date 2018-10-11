using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableTimespanValueRetrieverTests
    {
        [Test]
        public void Returns_an_nullable_timespan_when_passed_a_parsable_string_representation_of_a_timespan()
        {
            var retriever = new NullableTimeSpanValueRetriever();
            retriever.GetValue("20:00:00").Should().Be(System.TimeSpan.Parse("20:00:00"));
            retriever.GetValue("20:00:01").Should().Be(System.TimeSpan.Parse("20:00:01"));
            retriever.GetValue("00:00:00").Should().Be(System.TimeSpan.Parse("00:00:00"));
        }


        [Test]
        public void Returns_null_when_value_is_null()
        {
            var retriever = new NullableTimeSpanValueRetriever();
            retriever.GetValue(null).Should().Be(null);
        }

        [Test]
        public void Returns_null_when_string_is_empty()
        {
            var retriever = new NullableTimeSpanValueRetriever();
            retriever.GetValue(string.Empty).Should().Be(null);
        }

        [Test]
        public void It_handles_timespans()
        {
            IValueRetriever retriever = new NullableTimeSpanValueRetriever();
            var empty = new System.Collections.Generic.KeyValuePair<string, string>();
            retriever.CanRetrieve(empty, null, typeof(System.TimeSpan?)).Should().BeTrue();
            retriever.CanRetrieve(empty, null, typeof(System.TimeSpan)).Should().BeFalse();
            retriever.CanRetrieve(empty, null, typeof(System.String)).Should().BeFalse();
            retriever.CanRetrieve(empty, null, typeof(System.Boolean)).Should().BeFalse();
        }
    }
}