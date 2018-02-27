﻿using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class DateTimeOffsetValueRetrieverTests
	{
		[SetUp]
		public void TestSetup()
		{
			// this is required, because the tests depend on parsing decimals with the en-US culture
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		}

		[Test]
        public void Returns_MinValue_when_the_value_is_null()
        {
            var retriever = new DateTimeOffsetValueRetriever();
            retriever.GetValue(null).Should().Be(DateTimeOffset.MinValue);
        }

        [Test]
        public void Returns_MinValue_when_the_value_is_empty()
        {
            var retriever = new DateTimeOffsetValueRetriever();
            retriever.GetValue(string.Empty).Should().Be(DateTimeOffset.MinValue);
        }

        [Test]
        public void Returns_the_date_when_value_represents_a_valid_date()
        {
            var retriever = new DateTimeOffsetValueRetriever();
            var date1 = new DateTime(2011, 1, 1);
            retriever.GetValue("2011-01-01").Should().Be(new DateTimeOffset(2011, 1, 1, 0, 0, 0, TimeZone.CurrentTimeZone.GetUtcOffset(date1)));
            var date2 = new DateTime(2013, 12, 5);
            retriever.GetValue("2013-12-05").Should().Be(new DateTimeOffset(2013, 12, 5, 0, 0, 0, TimeZone.CurrentTimeZone.GetUtcOffset(date2)));
        }

		[Test]
        public void Returns_the_date_and_time_when_value_represents_a_valid_datetime()
        {
            var retriever = new DateTimeOffsetValueRetriever();
            var date1 = new DateTime(2011, 1, 1);
            retriever.GetValue("2011-01-01 15:16:17").Should().Be(new DateTimeOffset(2011, 1, 1, 15, 16, 17, TimeZone.CurrentTimeZone.GetUtcOffset(date1)));
            var date2 = new DateTime(2011, 1, 1);
            retriever.GetValue("2011-01-01 5:6:7").Should().Be(new DateTimeOffset(2011, 1, 1, 5, 6, 7, TimeZone.CurrentTimeZone.GetUtcOffset(date2)));
		}

	    [Test]
	    public void Returns_the_date_and_time_represents_a_valid_date_if_culture_is_fr_FR()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

			var retriever = new DateTimeOffsetValueRetriever();
			var date1 = new DateTime(2011, 5, 1);
		    retriever.GetValue("01/05/2011 15:16:17").Should().Be(new DateTimeOffset(2011, 5, 1, 15, 16, 17, TimeZone.CurrentTimeZone.GetUtcOffset(date1)));
		    var date2 = new DateTime(2011, 5, 1);
		    retriever.GetValue("01/05/2011 5:6:7").Should().Be(new DateTimeOffset(2011, 5, 1, 5, 6, 7, TimeZone.CurrentTimeZone.GetUtcOffset(date2)));
		}

		[Test]
        public void Returns_MinValue_when_the_value_is_not_a_valid_datetime()
        {
            var retriever = new DateTimeOffsetValueRetriever();
            retriever.GetValue("xxxx").Should().Be(DateTimeOffset.MinValue);
            retriever.GetValue("this is not a date").Should().Be(DateTimeOffset.MinValue);
            retriever.GetValue("Thursday").Should().Be(DateTimeOffset.MinValue);
            retriever.GetValue("Friday too").Should().Be(DateTimeOffset.MinValue);
        }
    }
}
