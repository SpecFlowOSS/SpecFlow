using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class ULongValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public ULongValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Theory]
        [InlineData(typeof(ulong), true)]
        [InlineData(typeof(ulong?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new ULongValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1UL)]
        [InlineData("3", 3UL)]
        [InlineData("1234567890123456789", 1234567890123456789UL)]
        [InlineData("1,234,567,890,123,456,789", 1234567890123456789UL)]
        [InlineData("x", 0UL)]
        [InlineData("-1", 0UL)]
        [InlineData("123456789019999923333333333333333456789", 0UL)]
        [InlineData("every good boy does fine", 0UL)]
        [InlineData(null, 0UL)]
        [InlineData("", 0UL)]
        public void Retrieve_correct_value(string value, ulong expectation)
        {
            var retriever = new ULongValueRetriever();
            var result = (ulong)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(ulong));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1UL)]
        [InlineData("3", 3UL)]
        [InlineData("1234567890123456789", 1234567890123456789UL)]
        [InlineData("1,234,567,890,123,456,789", 1234567890123456789UL)]
        [InlineData("x", 0UL)]
        [InlineData("-1", 0UL)]
        [InlineData("123456789019999923333333333333333456789", 0UL)]
        [InlineData("every good boy does fine", 0UL)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, ulong? expectation)
        {
            var retriever = new ULongValueRetriever();
            var result = (ulong?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(ulong?));
            result.Should().Be(expectation);
        }

        [Fact]
        public void Retrieve_a_ulong_when_passed_a_ulong_value_if_culture_is_fr_Fr()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new ULongValueRetriever();
            var result = (ulong?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, "1234567890123456789,0"), IrrelevantType, typeof(ulong?));
            result.Should().Be(1234567890123456789UL);
        }
    }
}