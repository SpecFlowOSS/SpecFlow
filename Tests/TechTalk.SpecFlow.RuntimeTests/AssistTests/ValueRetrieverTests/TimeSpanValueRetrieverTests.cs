using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class TimespanValueRetrieverTests
    {
        [Fact]
        public void Returns_a_timespan_when_passed_a_parsable_string_representation_of_a_timespan()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

            var retriever = new TimeSpanValueRetriever();
            retriever.GetValue("20:00:00").Should().Be(System.TimeSpan.Parse("20:00:00"));
            retriever.GetValue("20:00:01").Should().Be(System.TimeSpan.Parse("20:00:01"));
            retriever.GetValue("00:00:00").Should().Be(System.TimeSpan.Parse("00:00:00"));
            retriever.GetValue("6:12:14:45.3448").Should().Be(System.TimeSpan.Parse("6.12:14:45.3448000"));
        }

        [Fact]
        public void Returns_a_timespan_when_passed_a_parsable_string_representation_of_a_timespan_and_culture_is_fr_FR()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new TimeSpanValueRetriever();
            retriever.GetValue("6:12:14:45,3448").Should().Be(System.TimeSpan.Parse("6.12:14:45.3448000"));
        }

		[Fact]
        public void It_handles_timespans()
        {
            IValueRetriever retriever = new TimeSpanValueRetriever();
            var empty = new System.Collections.Generic.KeyValuePair<string, string>();
            retriever.CanRetrieve(empty, null, typeof(System.TimeSpan)).Should().BeTrue();
            retriever.CanRetrieve(empty, null, typeof(System.String)).Should().BeFalse();
            retriever.CanRetrieve(empty, null, typeof(System.Boolean)).Should().BeFalse();
        }
    }
}