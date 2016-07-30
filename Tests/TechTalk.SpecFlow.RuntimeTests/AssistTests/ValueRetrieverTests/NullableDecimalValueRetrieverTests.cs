using System;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class NullableDecimalValueRetrieverTests
    {
        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableDecimalValueRetriever(v => 23M);
            retriever.GetValue(null).Should().Be(null);
        }

        [Test]
        public void Returns_value_from_decimal_value_retriever_when_not_empty()
        {
            Func<string, decimal> func = v =>
                                             {
                                                 if (v == "value 1") return 1M;
                                                 if (v == "value 2") return 2M;
                                                 return 0M;
                                             };

            var retriever = new NullableDecimalValueRetriever(func);
            retriever.GetValue("value 1").Should().Be(1M);
            retriever.GetValue("value 2").Should().Be(2M);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableDecimalValueRetriever(v => 3M);
            retriever.GetValue("").Should().Be(null);
        }
    }
}