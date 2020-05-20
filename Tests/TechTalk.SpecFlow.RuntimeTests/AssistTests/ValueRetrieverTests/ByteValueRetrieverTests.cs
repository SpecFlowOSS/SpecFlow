using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using System.Globalization;
using System.Threading;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class ByteValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public ByteValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Theory]
        [InlineData(typeof(byte), true)]
        [InlineData(typeof(byte?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new ByteValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("3", 3)]
        [InlineData("30", 30)]
        [InlineData("x", 0)]
        [InlineData("-1", 0)]
        [InlineData("500", 0)]
        [InlineData("every good boy does fine", 0)]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        public void Retrieve_correct_value(string value, byte expectation)
        {
            var retriever = new ByteValueRetriever();
            var result = (byte)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(byte));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("1", (byte)1)]
        [InlineData("3", (byte)3)]
        [InlineData("30", (byte)30)]
        [InlineData("x", (byte)0)]
        [InlineData("-1", (byte)0)]
        [InlineData("500", (byte)0)]
        [InlineData("every good boy does fine", (byte)0)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, byte? expectation)
        {
            var retriever = new ByteValueRetriever();
            var result = (byte?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(byte?));
            result.Should().Be(expectation);
        }

        [Fact]
        public void Retrieve_a_byte_when_passed_a_byte_value_if_culture_is_fr_Fr()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new ByteValueRetriever();
            var result = (byte?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, "30,0"), IrrelevantType, typeof(byte?));
            result.Should().Be(30);
        }
    }
}