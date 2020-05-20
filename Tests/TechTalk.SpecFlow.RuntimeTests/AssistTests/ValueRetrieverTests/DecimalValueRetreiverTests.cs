using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class DecimalValueRetreiverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public DecimalValueRetreiverTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        [Theory]
        [InlineData(typeof(decimal), true)]
        [InlineData(typeof(decimal?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new DecimalValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("0", 0d)]
        [InlineData("1", 1d)]
        [InlineData("2", 2d)]
        [InlineData("2.23", 2.23d)]
        [InlineData("384.234879", 384.234879d)]
        [InlineData("-1", -1d)]
        [InlineData("-32.234", -32.234d)]
        [InlineData("xxxslkdfj.", 0d)]
        [InlineData(null, 0d)]
        [InlineData("", 0d)]
        public void Retrieve_correct_value(string value, decimal expectation)
        {
            var retriever = new DecimalValueRetriever();
            var result = (decimal)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(decimal));
            result.Should().Be(expectation);
        }

        [Theory]
        [InlineData("0", 0d)]
        [InlineData("1", 1d)]
        [InlineData("2", 2d)]
        [InlineData("2.23", 2.23d)]
        [InlineData("384.234879", 384.234879d)]
        [InlineData("-1", -1d)]
        [InlineData("-32.234", -32.234d)]
        [InlineData("xxxslkdfj.", 0d)]
        [InlineData(null, null)]
        [InlineData("", null)]
        public void Retrieve_correct_nullable_value(string value, double? expectation)
        {
            var retriever = new DecimalValueRetriever();
            var result = (decimal?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(decimal?));
            result.Should().Be((decimal?)expectation);
        }

        [Fact]
        public void Retrieve_a_decimal_when_passed_a_decimal_value_if_culture_is_fr_Fr()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            var retriever = new DecimalValueRetriever();
            var result = (decimal?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, "384,234879"), IrrelevantType, typeof(decimal?));
            result.Should().Be(384.234879m);
        }
    }
}