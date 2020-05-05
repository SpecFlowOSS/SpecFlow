using System;
using System.Globalization;
using System.Threading;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class DateTimeValueRetrieverTests
    {
        public DateTimeValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        [Fact]
        public void Returns_MinValue_when_the_value_is_null()
        {
            var retriever = new DateTimeValueRetriever();
            retriever.GetValue(null).Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void Returns_MinValue_when_the_value_is_empty()
        {
            var retriever = new DateTimeValueRetriever();
            retriever.GetValue(string.Empty).Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void Returns_the_date_when_value_represents_a_valid_date()
        {
            var retriever = new DateTimeValueRetriever();
            retriever.GetValue("1/1/2011").Should().Be(new DateTime(2011, 1, 1));
            retriever.GetValue("12/31/2015").Should().Be(new DateTime(2015, 12, 31));
        }

        [Fact]
        public void Returns_the_date_and_time_when_value_represents_a_valid_datetime()
        {
            var retriever = new DateTimeValueRetriever();
            retriever.GetValue("1/1/2011 15:16:17").Should().Be(new DateTime(2011, 1, 1, 15, 16, 17));
            retriever.GetValue("1/1/2011 5:6:7").Should().Be(new DateTime(2011, 1, 1, 5, 6, 7));
        }

        [Fact]
	    public void Returns_the_date_and_time_when_value_represents_a_valid_datetime_if_culture_is_fr_FR()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

			var retriever = new DateTimeValueRetriever();
		    retriever.GetValue("1/4/2011 15:16:17").Should().Be(new DateTime(2011, 4, 1, 15, 16, 17));
		    retriever.GetValue("1/4/2011 5:6:7").Should().Be(new DateTime(2011, 4, 1, 5, 6, 7));
	    }

		[Fact]
        public void Returns_MinValue_when_the_value_is_not_a_valid_datetime()
        {
            var retriever = new DateTimeValueRetriever();
            retriever.GetValue("xxxx").Should().Be(DateTime.MinValue);
            retriever.GetValue("this is not a date").Should().Be(DateTime.MinValue);
            retriever.GetValue("Thursday").Should().Be(DateTime.MinValue);
        }
    }
}