using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class TimespanValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public TimespanValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Theory]
        [InlineData(typeof(TimeSpan), true)]
        [InlineData(typeof(TimeSpan?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new TimeSpanValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("20:00:00")]
        [InlineData("20:00:01")]
        [InlineData("00:00:00")]
        [InlineData("6:12:14:45.3448")]
        public void Retrieve_correct_value(string value)
        {
            var retriever = new TimeSpanValueRetriever();
            var result = (TimeSpan)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(TimeSpan));
            result.Should().Be(TimeSpan.Parse(value));
        }

        [Theory]
        [InlineData("20:00:00")]
        [InlineData("20:00:01")]
        [InlineData("00:00:00")]
        [InlineData("6:12:14:45.3448")]
        public void Retrieve_correct_nullable_value(string value)
        {
            var retriever = new TimeSpanValueRetriever();
            var result = (TimeSpan?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(TimeSpan?));
            result.Should().Be(TimeSpan.Parse(value));
        }

        [Theory]
        [InlineData("123456789019999923456789")]
        [InlineData("every good boy does fine")]
        [InlineData(null)]
        [InlineData("")]
        public void Retrieve_Zero_for_illegal_values(string value)
        {
            var retriever = new TimeSpanValueRetriever();
            var result = (TimeSpan)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(TimeSpan));
            result.Should().Be(TimeSpan.Zero);
        }

        [Theory]
        [InlineData("123456789019999923456789")]
        [InlineData("every good boy does fine")]
        public void Retrieve_Zero_for_illegal_values_when_nullable_TimeSpan(string value)
        {
            var retriever = new TimeSpanValueRetriever();
            var result = (TimeSpan?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(TimeSpan?));
            result.Should().Be(TimeSpan.Zero);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Retrieve_null_for_nullable_value_when_nullable_TimeSpan(string value)
        {
            var retriever = new TimeSpanValueRetriever();
            var result = (TimeSpan?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(TimeSpan?));
            result.Should().Be(null);
        }

        [Fact]
        public void Retrieve_a_TimeSpan_when_passed_a_TimeSpan_value_if_culture_is_fr_Fr()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new TimeSpanValueRetriever();
            var result = (TimeSpan?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, "6:12:14:45,3448"), IrrelevantType, typeof(TimeSpan?));
            result.Should().Be(TimeSpan.Parse("6.12:14:45.3448000"));
        }
    }
}