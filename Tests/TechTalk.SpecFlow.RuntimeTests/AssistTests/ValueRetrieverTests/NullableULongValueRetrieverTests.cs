using System;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class NullableULongValueRetrieverTests
    {
        [Fact]
        public void Returns_null_when_the_value_is_null()
        {
            var retriever = new NullableULongValueRetriever(v => 0);
            retriever.GetValue(null).Should().Be(null);
        }

        [Fact]
        public void Returns_value_from_ULongValueRetriever_when_passed_not_empty_string()
        {
            Func<string, ulong> func = v =>
            {
                if (v == "test value") return 123;
                if (v == "another test value") return 456;
                return 0;
            };

            var retriever = new NullableULongValueRetriever(func);
            retriever.GetValue("test value").Should().Be((ulong?)123);
            retriever.GetValue("another test value").Should().Be((ulong?)456);
        }

        [Fact]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableULongValueRetriever(v => 3);
            retriever.GetValue(string.Empty).Should().Be(null);
        }
    }
}