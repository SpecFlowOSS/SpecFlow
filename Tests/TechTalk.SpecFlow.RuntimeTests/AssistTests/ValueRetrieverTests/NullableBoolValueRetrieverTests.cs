using System;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableBoolValueRetrieverTests
    {
        [Test]
        public void Returns_the_value_from_the_BoolValueRetriever()
        {
            Func<string, bool> func = value => value == "this value" || value == "another value";
            
            var retriever = new NullableBoolValueRetriever(func);
            retriever.GetValue("this value").Should().Be(true);
            retriever.GetValue("another value").Should().Be(true);
            retriever.GetValue("failing value").Should().Be(false);
            retriever.GetValue("another thing that returns false").Should().Be(false);
        }

        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableBoolValueRetriever(value => true);
            retriever.GetValue(null).Should().Be(null);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableBoolValueRetriever(value => true);
            retriever.GetValue(string.Empty).Should().Be(null);
        }
    }
}