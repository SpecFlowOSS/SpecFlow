using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class SByteValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public SByteValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Theory]
        [InlineData(typeof(sbyte), true)]
        [InlineData(typeof(sbyte?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new SByteValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("3", 3)]
        [InlineData("30", 30)]
        [InlineData("x", 0)]
        [InlineData("-1", -1)]
        [InlineData("-5", -5)]
        [InlineData("500", 0)]
        [InlineData("every good boy does fine", 0)]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        public void Retrieve_correct_value(string value, sbyte expectation)
        {
            var retriever = new SByteValueRetriever();
            var result = (sbyte)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(sbyte));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", (sbyte)1)]
        [InlineData("3", (sbyte)3)]
        [InlineData("30", (sbyte)30)]
        [InlineData("x", (sbyte)0)]
        [InlineData("-1", (sbyte)-1)]
        [InlineData("-5", (sbyte)-5)]
        [InlineData("500", (sbyte)0)]
        [InlineData("every good boy does fine", (sbyte)0)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, sbyte? expectation)
        {
            var retriever = new SByteValueRetriever();
            var result = (sbyte?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(sbyte?));
            result.Should().Be(expectation);
        }

        [Fact]
        public void Retrieve_a_sbyte_when_passed_a_sbyte_value_if_culture_is_fr_Fr()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new SByteValueRetriever();
            var result = (sbyte?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, "30,0"), IrrelevantType, typeof(sbyte?));
            result.Should().Be(30);
        }
    }
}