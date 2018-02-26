using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class TimespanValueRetrieverTests
    {
        [Fact]
        public void Returns_a_timespan_when_passed_a_parsable_string_representation_of_a_timespan()
        {
            var retriever = new TimeSpanValueRetriever();
            retriever.GetValue("20:00:00").Should().Be(System.TimeSpan.Parse("20:00:00"));
            retriever.GetValue("20:00:01").Should().Be(System.TimeSpan.Parse("20:00:01"));
            retriever.GetValue("00:00:00").Should().Be(System.TimeSpan.Parse("00:00:00"));
        }

        [Fact]
        public void It_handles_timespans()
        {
            var retriever = new TimeSpanValueRetriever();
            var empty = new System.Collections.Generic.KeyValuePair<string, string>();
            retriever.CanRetrieve(empty, null, typeof(System.TimeSpan)).Should().BeTrue();
            retriever.CanRetrieve(empty, null, typeof(System.String)).Should().BeFalse();
            retriever.CanRetrieve(empty, null, typeof(System.Boolean)).Should().BeFalse();
        }
    }
}