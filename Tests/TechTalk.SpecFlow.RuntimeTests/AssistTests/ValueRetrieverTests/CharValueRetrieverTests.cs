using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class CharValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        [Theory]
        [InlineData(typeof(char), true)]
        [InlineData(typeof(char?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new CharValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("a", 'a')]
        [InlineData("A", 'A')]
        [InlineData("1", '1')]
        [InlineData("&", '&')]
        [InlineData(" ", ' ')]
        [InlineData("ab", '\0')]
        [InlineData("abc", '\0')]
        [InlineData("abcdefg.", '\0')]
        [InlineData(null, '\0')]
        [InlineData("", '\0')]
        public void Retrieve_correct_value(string value, char expectation)
        {
            var retriever = new CharValueRetriever();
            var result = (char)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(char));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("a", 'a')]
        [InlineData("A", 'A')]
        [InlineData("1", '1')]
        [InlineData("&", '&')]
        [InlineData(" ", ' ')]
        [InlineData("ab", '\0')]
        [InlineData("abc", '\0')]
        [InlineData("abcdefg.", '\0')]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, char? expectation)
        {
            var retriever = new CharValueRetriever();
            var result = (char?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(char?));
            result.Should().Be(expectation);
        }
    }
}