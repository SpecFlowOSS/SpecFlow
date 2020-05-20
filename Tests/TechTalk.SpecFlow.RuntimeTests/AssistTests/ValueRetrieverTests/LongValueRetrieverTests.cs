using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class LongValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public LongValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Theory]
        [InlineData(typeof(long), true)]
        [InlineData(typeof(long?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new LongValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1L)]
        [InlineData("3", 3L)]
        [InlineData("1234567890123456789", 1234567890123456789L)]
        [InlineData("1,234,567,890,123,456,789", 1234567890123456789L)]
        [InlineData("x", 0L)]
        [InlineData("-1", -1L)]
        [InlineData("-5", -5L)]
        [InlineData("123456789019999923333333333333333456789", 0L)]
        [InlineData("every good boy does fine", 0L)]
        [InlineData(null, 0L)]
        [InlineData("", 0L)]
        public void Retrieve_correct_value(string value, long expectation)
        {
            var retriever = new LongValueRetriever();
            var result = (long)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(long));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1L)]
        [InlineData("3", 3L)]
        [InlineData("1234567890123456789", 1234567890123456789L)]
        [InlineData("1,234,567,890,123,456,789", 1234567890123456789L)]
        [InlineData("x", 0L)]
        [InlineData("-1", -1L)]
        [InlineData("-5", -5L)]
        [InlineData("123456789019999923333333333333333456789", 0L)]
        [InlineData("every good boy does fine", 0L)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, long? expectation)
        {
            var retriever = new LongValueRetriever();
            var result = (long?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(long?));
            result.Should().Be(expectation);
        }

        [Fact]
        public void Retrieve_a_long_when_passed_a_long_value_if_culture_is_fr_Fr()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new LongValueRetriever();
            var result = (long?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, "1234567890123456789,0"), IrrelevantType, typeof(long?));
            result.Should().Be(1234567890123456789L);
        }
	}
}