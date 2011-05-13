using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class DateTimeValueRetrieverTests
    {
        [Test]
        public void Returns_MinValue_when_the_value_is_null()
        {
            var retriever = new DateTimeValueRetriever();
            retriever.GetValue(null).ShouldEqual(DateTime.MinValue);
        }

        [Test]
        public void Returns_MinValue_when_the_value_is_empty()
        {
            var retriever = new DateTimeValueRetriever();
            retriever.GetValue(string.Empty).ShouldEqual(DateTime.MinValue);
        }

        [Test]
        public void Returns_the_date_when_value_represents_a_valid_date()
        {
            var retriever = new DateTimeValueRetriever();
            retriever.GetValue("1/1/2011").ShouldEqual(new DateTime(2011, 1, 1));
            retriever.GetValue("12/31/2015").ShouldEqual(new DateTime(2015, 12, 31));
        }

        [Test]
        public void Returns_the_date_and_time_when_value_represents_a_valid_datetime()
        {
            var retriever = new DateTimeValueRetriever();
            retriever.GetValue("1/1/2011 15:16:17").ShouldEqual(new DateTime(2011, 1, 1, 15, 16, 17));
            retriever.GetValue("1/1/2011 5:6:7").ShouldEqual(new DateTime(2011, 1, 1, 5, 6, 7));
        }

        [Test]
        public void Returns_MinValue_when_the_value_is_not_a_valid_datetime()
        {
            var retriever = new DateTimeValueRetriever();
            retriever.GetValue("xxxx").ShouldEqual(DateTime.MinValue);
            retriever.GetValue("this is not a date").ShouldEqual(DateTime.MinValue);
            retriever.GetValue("Thursday").ShouldEqual(DateTime.MinValue);
        }
    }
}