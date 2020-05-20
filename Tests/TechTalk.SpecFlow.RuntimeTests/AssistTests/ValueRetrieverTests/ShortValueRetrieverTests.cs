using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class ShortValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public ShortValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Theory]
        [InlineData(typeof(short), true)]
        [InlineData(typeof(short?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new ShortValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("3", 3)]
        [InlineData("12,345", 12345)]
        [InlineData("x", 0)]
        [InlineData("-1", -1)]
        [InlineData("-5", -5)]
        [InlineData("1234567890123456789", 0)]
        [InlineData("every good boy does fine", 0)]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        public void Retrieve_correct_value(string value, short expectation)
        {
            var retriever = new ShortValueRetriever();
            var result = (short)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(short));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", (short)1)]
        [InlineData("3", (short)3)]
        [InlineData("30", (short)30)]
        [InlineData("x", (short)0)]
        [InlineData("-1", (short)-1)]
        [InlineData("-5", (short)-5)]
        [InlineData("1234567890123456789", (short)0)]
        [InlineData("every good boy does fine", (short)0)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, short? expectation)
        {
            var retriever = new ShortValueRetriever();
            var result = (short?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(short?));
            result.Should().Be(expectation);
        }

        [Fact]
        public void Retrieve_a_short_when_passed_a_short_value_if_culture_is_fr_Fr()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new ShortValueRetriever();
            var result = (short?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, "305,0"), IrrelevantType, typeof(short?));
            result.Should().Be(305);
        }
    }
}