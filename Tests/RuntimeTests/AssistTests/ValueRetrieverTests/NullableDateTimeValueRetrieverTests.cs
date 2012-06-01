using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class NullableDateTimeValueRetrieverTests : AssistTestsBase
    {
        [Test]
        public void Returns_the_value_from_the_DateTimeValueRetriever()
        {
            var retriever = new NullableDateTimeValueRetriever();
            retriever.GetValue("2011-1-2").ShouldEqual(new DateTime(2011, 1, 2));
            retriever.GetValue("2015-12-31").ShouldEqual(new DateTime(2015, 12, 31));
        }

        [Test]
        public void Returns_null_when_value_is_null()
        {
            var retriever = new NullableDateTimeValueRetriever();
            retriever.GetValue((string) null).ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_string_is_empty()
        {
            var retriever = new NullableDateTimeValueRetriever();
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}