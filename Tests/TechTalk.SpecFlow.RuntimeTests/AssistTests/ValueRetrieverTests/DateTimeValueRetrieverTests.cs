using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class DateTimeValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);

        public DateTimeValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        [Theory]
        [InlineData(typeof(DateTime), true)]
        [InlineData(typeof(DateTime?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new DateTimeValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }

        private void Retrieve_correct_value(string value, DateTime expectation)
        {
            var retriever = new DateTimeValueRetriever();
            var result = (DateTime)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(DateTime));
            result.Should().Be(expectation);
        }

        private void Retrieve_correct_nullable_value(string value, DateTime? expectation)
        {
            var retriever = new DateTimeValueRetriever();
            var result = (DateTime?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(DateTime?));
            result.Should().Be(expectation);
        }

        [Fact]
        public void Returns_MinValue_when_the_value_is_null()
        {
            Retrieve_correct_value(null, DateTime.MinValue);
        }

        [Fact]
        public void Returns_null_when_the_value_is_null_and_type_nullable()
        {
            Retrieve_correct_nullable_value(null, null);
        }

        [Fact]
        public void Returns_MinValue_when_the_value_is_empty()
        {
            Retrieve_correct_value(string.Empty, DateTime.MinValue);
        }

        [Fact]
        public void Returns_null_when_the_value_is_empty_and_type_nullable()
        {
            Retrieve_correct_nullable_value(string.Empty, null);
        }

        [Fact]
        public void Returns_the_date_when_value_represents_a_valid_date()
        {
            Retrieve_correct_value("1/1/2011", new DateTime(2011, 1, 1));
            Retrieve_correct_nullable_value("1/1/2011", new DateTime(2011, 1, 1));
            Retrieve_correct_value("12/31/2015", new DateTime(2015, 12, 31));
            Retrieve_correct_nullable_value("12/31/2015", new DateTime(2015, 12, 31));
        }

        [Fact]
        public void Returns_the_date_and_time_when_value_represents_a_valid_datetime()
        {
            Retrieve_correct_value("1/1/2011 15:16:17", new DateTime(2011, 1, 1, 15, 16, 17));
            Retrieve_correct_nullable_value("1/1/2011 15:16:17", new DateTime(2011, 1, 1, 15, 16, 17));
            Retrieve_correct_value("1/1/2011 5:6:7", new DateTime(2011, 1, 1, 5, 6, 7));
            Retrieve_correct_nullable_value("1/1/2011 5:6:7", new DateTime(2011, 1, 1, 5, 6, 7));
        }

        [Fact]
        public void Returns_the_date_and_time_represents_a_valid_date_if_culture_is_fr_FR()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

            Retrieve_correct_value("01/04/2011 15:16:17", new DateTime(2011, 4, 1, 15, 16, 17));
            Retrieve_correct_nullable_value("01/04/2011 15:16:17", new DateTime(2011, 4, 1, 15, 16, 17));
            Retrieve_correct_value("01/04/2011 5:6:7", new DateTime(2011, 4, 1, 5, 6, 7));
            Retrieve_correct_nullable_value("01/04/2011 5:6:7", new DateTime(2011, 4, 1, 5, 6, 7));
        }

        [Fact]
        public void Returns_MinValue_when_the_value_is_not_a_valid_datetime()
        {
            Retrieve_correct_value("xxxx", DateTime.MinValue);
            Retrieve_correct_nullable_value("xxxx", DateTime.MinValue);
            Retrieve_correct_value("this is not a date", DateTime.MinValue);
            Retrieve_correct_nullable_value("this is not a date", DateTime.MinValue);
            Retrieve_correct_value("Thursday", DateTime.MinValue);
            Retrieve_correct_nullable_value("Thursday", DateTime.MinValue);
        }
    }
}