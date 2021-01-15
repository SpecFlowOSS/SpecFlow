using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class DateTimeOffsetValueRetrieverTests
    {
        private const string IrrelevantKey = "Irrelevant";
        private readonly Type IrrelevantType = typeof(object);
        
        public DateTimeOffsetValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        [Theory]
        [InlineData(typeof(DateTimeOffset), true)]
        [InlineData(typeof(DateTimeOffset?), true)]
        [InlineData(typeof(int), false)]
        public void CanRetrieve(Type type, bool expectation)
        {
            var retriever = new DateTimeOffsetValueRetriever();
            var result = retriever.CanRetrieve(new KeyValuePair<string, string>(IrrelevantKey, IrrelevantKey), IrrelevantType, type);
            result.Should().Be(expectation);
        }
        private void Retrieve_correct_value(string value, DateTimeOffset expectation)
        {
            var retriever = new DateTimeOffsetValueRetriever();
            var result = (DateTimeOffset)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(DateTimeOffset));
            result.Should().Be(expectation);
        }

        private void Retrieve_correct_nullable_value(string value, DateTimeOffset? expectation)
        {
            var retriever = new DateTimeOffsetValueRetriever();
            var result = (DateTimeOffset?)retriever.Retrieve(new KeyValuePair<string, string>(IrrelevantKey, value), IrrelevantType, typeof(DateTimeOffset?));
            result.Should().Be(expectation);
        }

        [Fact]
        public void Returns_MinValue_when_the_value_is_null()
        {
            Retrieve_correct_value(null, DateTimeOffset.MinValue);
        }

        [Fact]
        public void Returns_null_when_the_value_is_null_and_type_nullable()
        {
            Retrieve_correct_nullable_value(null, null);
        }

        [Fact]
        public void Returns_MinValue_when_the_value_is_empty()
        {
            Retrieve_correct_value(string.Empty, DateTimeOffset.MinValue);
        }

        [Fact]
        public void Returns_null_when_the_value_is_empty_and_type_nullable()
        {
            Retrieve_correct_nullable_value(string.Empty, null);
        }

        [Fact]
        public void Returns_the_date_when_value_represents_a_valid_date()
        {
            var date1 = new DateTime(2011, 1, 1);
            Retrieve_correct_value("2011-01-01", new DateTimeOffset(2011, 1, 1, 0, 0, 0, TimeZoneInfo.Local.GetUtcOffset(date1)));
            Retrieve_correct_nullable_value("2011-01-01", new DateTimeOffset(2011, 1, 1, 0, 0, 0, TimeZoneInfo.Local.GetUtcOffset(date1)));
            var date2 = new DateTime(2013, 12, 5);
            Retrieve_correct_value("2013-12-05", new DateTimeOffset(2013, 12, 5, 0, 0, 0, TimeZoneInfo.Local.GetUtcOffset(date2)));
            Retrieve_correct_nullable_value("2013-12-05", new DateTimeOffset(2013, 12, 5, 0, 0, 0, TimeZoneInfo.Local.GetUtcOffset(date2)));
        }

        [Fact]
        public void Returns_the_date_and_time_when_value_represents_a_valid_datetime()
        {
            var date1 = new DateTime(2011, 1, 1);
            Retrieve_correct_value("2011-01-01 15:16:17", new DateTimeOffset(2011, 1, 1, 15, 16, 17, TimeZoneInfo.Local.GetUtcOffset(date1)));
            Retrieve_correct_nullable_value("2011-01-01 15:16:17", new DateTimeOffset(2011, 1, 1, 15, 16, 17, TimeZoneInfo.Local.GetUtcOffset(date1)));
            var date2 = new DateTime(2011, 1, 1);
            Retrieve_correct_value("2011-01-01 5:6:7", new DateTimeOffset(2011, 1, 1, 5, 6, 7, TimeZoneInfo.Local.GetUtcOffset(date2)));
            Retrieve_correct_nullable_value("2011-01-01 5:6:7", new DateTimeOffset(2011, 1, 1, 5, 6, 7, TimeZoneInfo.Local.GetUtcOffset(date2)));
        }

        [Fact]
	    public void Returns_the_date_and_time_represents_a_valid_date_if_culture_is_fr_FR()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

			var date1 = new DateTime(2011, 5, 1);
            Retrieve_correct_value("01/05/2011 15:16:17", new DateTimeOffset(2011, 5, 1, 15, 16, 17, TimeZoneInfo.Local.GetUtcOffset(date1)));
            Retrieve_correct_nullable_value("01/05/2011 15:16:17", new DateTimeOffset(2011, 5, 1, 15, 16, 17, TimeZoneInfo.Local.GetUtcOffset(date1)));
		    var date2 = new DateTime(2011, 5, 1);
            Retrieve_correct_value("01/05/2011 5:6:7", new DateTimeOffset(2011, 5, 1, 5, 6, 7, TimeZoneInfo.Local.GetUtcOffset(date2)));
            Retrieve_correct_nullable_value("01/05/2011 5:6:7", new DateTimeOffset(2011, 5, 1, 5, 6, 7, TimeZoneInfo.Local.GetUtcOffset(date2)));
		}

		[Fact]
        public void Returns_MinValue_when_the_value_is_not_a_valid_datetime()
        {
            Retrieve_correct_value("xxxx", DateTimeOffset.MinValue);
            Retrieve_correct_nullable_value("xxxx", DateTimeOffset.MinValue);
            Retrieve_correct_value("this is not a date", DateTimeOffset.MinValue);
            Retrieve_correct_nullable_value("this is not a date", DateTimeOffset.MinValue);
            Retrieve_correct_value("Thursday", DateTimeOffset.MinValue);
            Retrieve_correct_nullable_value("Thursday", DateTimeOffset.MinValue);
            Retrieve_correct_value("Friday too", DateTimeOffset.MinValue);
            Retrieve_correct_nullable_value("Friday too", DateTimeOffset.MinValue);
        }
    }
}
