using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class UIntValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public UIntValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Theory]
        [InlineData(typeof(uint), true)]
        [InlineData(typeof(uint?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new UIntValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1u)]
        [InlineData("3", 3u)]
        [InlineData("1234567890", 1234567890u)]
        [InlineData("1,234,567,890", 1234567890u)]
        [InlineData("x", 0u)]
        [InlineData("-1", 0u)]
        [InlineData("123456789019999923456789", 0u)]
        [InlineData("every good boy does fine", 0u)]
        [InlineData(null, 0u)]
        [InlineData("", 0u)]
        public void Retrieve_correct_value(string value, uint expectation)
        {
            var retriever = new UIntValueRetriever();
            var result = (uint)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(uint));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1u)]
        [InlineData("3", 3u)]
        [InlineData("1234567890", 1234567890u)]
        [InlineData("1,234,567,890", 1234567890u)]
        [InlineData("x", 0u)]
        [InlineData("-1", 0u)]
        [InlineData("123456789019999923456789", 0u)]
        [InlineData("every good boy does fine", 0u)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, uint? expectation)
        {
            var retriever = new UIntValueRetriever();
            var result = (uint?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(uint?));
            result.Should().Be(expectation);
        }

        [Fact]
        public void Retrieve_a_uint_when_passed_a_uint_value_if_culture_is_fr_Fr()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new UIntValueRetriever();
            var result = (uint?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, "1234567890,0"), IrrelevantType, typeof(uint?));
            result.Should().Be(1234567890u);
        }
    }
}