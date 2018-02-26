using System;
using System.Globalization;
using System.Threading;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class NullableDateTimeValueRetrieverTests
    {
        public NullableDateTimeValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        [Fact]
        public void Returns_the_value_from_the_DateTimeValueRetriever()
        {
            Func<string, DateTime> func = v =>
                                              {
                                                  if (v == "one") return new DateTime(2011, 1, 2);
                                                  if (v == "two") return new DateTime(2015, 12, 31);
                                                  return DateTime.MinValue;
                                              };

            var retriever = new NullableDateTimeValueRetriever(func);
            retriever.GetValue("one").Should().Be(new DateTime(2011, 1, 2));
            retriever.GetValue("two").Should().Be(new DateTime(2015, 12, 31));
        }

        [Fact]
        public void Returns_null_when_value_is_null()
        {
            var retriever = new NullableDateTimeValueRetriever(v => DateTime.Parse("1/1/2016"));
            retriever.GetValue(null).Should().Be(null);
        }

        [Fact]
        public void Returns_null_when_string_is_empty()
        {
            var retriever = new NullableDateTimeValueRetriever(v => DateTime.Parse("1/1/2017"));
            retriever.GetValue(string.Empty).Should().Be(null);
        }
    }
}