using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class NullableDateTimeValueRetrieverTests
    {
        [Test]
        public void Returns_the_value_from_the_DateTimeValueRetriever()
        {
            Func<string, DateTime> func = v =>
                                              {
                                                  if (v == "one") return new DateTime(2011, 1, 2);
                                                  if (v == "two") return new DateTime(2015, 12, 31);
                                                  return DateTime.MinValue;
                                              };

            var retriever = new NullableDateTimeValueRetriever(func);
            retriever.GetValue("one").ShouldEqual(new DateTime(2011, 1, 2));
            retriever.GetValue("two").ShouldEqual(new DateTime(2015, 12, 31));
        }

        [Test]
        public void Returns_null_when_value_is_null()
        {
            var retriever = new NullableDateTimeValueRetriever(v => DateTime.Parse("1/1/2016"));
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_string_is_empty()
        {
            var retriever = new NullableDateTimeValueRetriever(v => DateTime.Parse("1/1/2017"));
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}