using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class IntValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public IntValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Theory]
        [InlineData(typeof(int), true)]
        [InlineData(typeof(int?), true)]
        [InlineData(typeof(long), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new IntValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("3", 3)]
        [InlineData("1234567890", 1234567890)]
        [InlineData("1,234,567,890", 1234567890)]
        [InlineData("x", 0)]
        [InlineData("-1", -1)]
        [InlineData("-5", -5)]
        [InlineData("123456789019999923456789", 0)]
        [InlineData("every good boy does fine", 0)]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        public void Retrieve_correct_value(string value, int expectation)
        {
            var retriever = new IntValueRetriever();
            var result = (int)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(int));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("3", 3)]
        [InlineData("1234567890", 1234567890)]
        [InlineData("1,234,567,890", 1234567890)]
        [InlineData("x", 0)]
        [InlineData("-1", -1)]
        [InlineData("-5", -5)]
        [InlineData("123456789019999923456789", 0)]
        [InlineData("every good boy does fine", 0)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, int? expectation)
        {
            var retriever = new IntValueRetriever();
            var result = (int?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(int?));
            result.Should().Be(expectation);
        }

        [Fact]
        public void Retrieve_a_int_when_passed_a_int_value_if_culture_is_fr_Fr()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new IntValueRetriever();
            var result = (int?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, "1234567890,0"), IrrelevantType, typeof(int?));
            result.Should().Be(1234567890);
        }
	}
}