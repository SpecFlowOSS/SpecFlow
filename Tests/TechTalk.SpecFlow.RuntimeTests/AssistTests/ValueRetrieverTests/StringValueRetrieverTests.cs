using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class StringValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        [Theory]
        [InlineData(typeof(string), true)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(object), false)]
        [InlineData(typeof(Uri), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new StringValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("every good boy does fine", "every good boy does fine")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void Retrieve_correct_value(string value, string expectation)
        {
            var retriever = new StringValueRetriever();
            var result = (string)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(string));
            result.Should().Be(expectation);
        }
    }
}