using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class NullableDateTimeOffsetValueRetrieverTests
    {
        [Test]
        public void Returns_the_value_from_the_DateTimeOffsetValueRetriever()
        {
            Func<string, DateTimeOffset> func = v =>
            {
                if (v == "one") return new DateTimeOffset(2011, 1, 2, 2, 0, 0, TimeSpan.Zero);
                if (v == "two") return new DateTimeOffset(2015, 12, 31, 2, 0, 0, TimeSpan.Zero);
                return DateTimeOffset.MinValue;
            };

            var retriever = new NullableDateTimeOffsetValueRetriever(func);
            retriever.GetValue("one").ShouldEqual(new DateTimeOffset(2011, 1, 2, 2, 0, 0, TimeSpan.Zero));
            retriever.GetValue("two").ShouldEqual(new DateTimeOffset(2015, 12, 31, 2, 0, 0, TimeSpan.Zero));
        }

        [Test]
        public void Returns_null_when_value_is_null()
        {
            var retriever = new NullableDateTimeOffsetValueRetriever(v => DateTimeOffset.Parse("1/1/2016"));
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_string_is_empty()
        {
            var retriever = new NullableDateTimeOffsetValueRetriever(v => DateTimeOffset.Parse("1/1/2017"));
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}