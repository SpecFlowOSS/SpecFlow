using System;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class NullableUShortValueRetrieverTests
    {
        [Fact]
        public void Returns_null_when_the_value_is_null()
        {
            var retriever = new NullableUShortValueRetriever(v => 0);
            retriever.GetValue(null).Should().Be(null);
        }

        [Fact]
        public void Returns_value_from_UShortValueRetriever_when_passed_not_empty_string()
        {
            Func<string, ushort> func = v =>
            {
                if (v == "test value") return 123;
                if (v == "another test value") return 456;
                return 0;
            };

            var retriever = new NullableUShortValueRetriever(func);
            retriever.GetValue("test value").Should().Be((ushort?)123);
            retriever.GetValue("another test value").Should().Be((ushort?)456);
        }

        [Fact]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableUShortValueRetriever(v => 3);
            retriever.GetValue(string.Empty).Should().Be(null);
        }
    }
}