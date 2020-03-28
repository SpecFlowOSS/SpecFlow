using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class BoolValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        [Theory]
        [InlineData(typeof(bool), true)]
        [InlineData(typeof(bool?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new BoolValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("True", true)]
        [InlineData("true", true)]
        [InlineData("1", true)]
        [InlineData("False", false)]
        [InlineData("false", false)]
        [InlineData("0", false)]
        [InlineData("sssssdfsd", false)]
        [InlineData("this is false", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void Retrieve_correct_value(string value, bool expectation)
        {
            var retriever = new BoolValueRetriever();
            var result = (bool)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(bool));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("True", true)]
        [InlineData("true", true)]
        [InlineData("1", true)]
        [InlineData("False", false)]
        [InlineData("false", false)]
        [InlineData("0", false)]
        [InlineData("sssssdfsd", false)]
        [InlineData("this is false", false)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, bool? expectation)
        {
            var retriever = new BoolValueRetriever();
            var result = (bool?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(bool?));
            result.Should().Be(expectation);
        }
    }
}