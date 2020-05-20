using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class UShortValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public UShortValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Theory]
        [InlineData(typeof(ushort), true)]
        [InlineData(typeof(ushort?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new UShortValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("3", 3)]
        [InlineData("12,345", 12345)]
        [InlineData("x", 0)]
        [InlineData("-1", 0)]
        [InlineData("1234567890123456789", 0)]
        [InlineData("every good boy does fine", 0)]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        public void Retrieve_correct_value(string value, ushort expectation)
        {
            var retriever = new UShortValueRetriever();
            var result = (ushort)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(ushort));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", (ushort)1)]
        [InlineData("3", (ushort)3)]
        [InlineData("30", (ushort)30)]
        [InlineData("x", (ushort)0)]
        [InlineData("-1", (ushort)0)]
        [InlineData("1234567890123456789", (ushort)0)]
        [InlineData("every good boy does fine", (ushort)0)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, ushort? expectation)
        {
            var retriever = new UShortValueRetriever();
            var result = (ushort?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(ushort?));
            result.Should().Be(expectation);
        }

        [Fact]
        public void Retrieve_a_ushort_when_passed_a_ushort_value_if_culture_is_fr_Fr()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new UShortValueRetriever();
            var result = (ushort?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, "305,0"), IrrelevantType, typeof(ushort?));
            result.Should().Be(305);
        }
    }
}