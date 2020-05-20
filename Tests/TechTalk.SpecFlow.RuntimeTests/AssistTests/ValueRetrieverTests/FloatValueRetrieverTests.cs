using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class FloatValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public FloatValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        [Theory]
        [InlineData(typeof(float), true)]
        [InlineData(typeof(float?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new FloatValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("0", 0f)]
        [InlineData("1", 1f)]
        [InlineData("2", 2f)]
        [InlineData("2.23", 2.23f)]
        [InlineData("384.234879", 384.234879f)]
        [InlineData("-1", -1f)]
        [InlineData("-32.234", -32.234f)]
        [InlineData("xxxslkdfj.", 0f)]
        [InlineData(null, 0f)]
        [InlineData("", 0f)]
        public void Retrieve_correct_value(string value, float expectation)
        {
            var retriever = new FloatValueRetriever();
            var result = (float)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(float));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("0", 0f)]
        [InlineData("1", 1f)]
        [InlineData("2", 2f)]
        [InlineData("2.23", 2.23f)]
        [InlineData("384.234879", 384.234879f)]
        [InlineData("-1", -1f)]
        [InlineData("-32.234", -32.234f)]
        [InlineData("xxxslkdfj.", 0f)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, float? expectation)
        {
            var retriever = new FloatValueRetriever();
            var result = (float?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(float?));
            result.Should().Be(expectation);
        }

        [Fact]
        public void Retrieve_a_float_when_passed_a_float_value_if_culture_is_fr_Fr()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new FloatValueRetriever();
            var result = (float?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, "384,234879"), IrrelevantType, typeof(float?));
            result.Should().Be(384.234879f);
        }
    }
}