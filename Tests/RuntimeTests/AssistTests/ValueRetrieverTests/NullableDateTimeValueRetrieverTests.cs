using System;
using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableDateTimeValueRetrieverTests
    {
        [Test]
        public void Returns_the_value_from_the_DateTimeValueRetriever()
        {
            var mock = new Mock<DateTimeValueRetriever>();
            mock.Setup(x => x.GetValue("one")).Returns(new DateTime(2011, 1, 2));
            mock.Setup(x => x.GetValue("two")).Returns(new DateTime(2015, 12, 31));

            var retriever = new NullableDateTimeValueRetriever(mock.Object);
            retriever.GetValue("one").ShouldEqual(new DateTime(2011, 1, 2));
            retriever.GetValue("two").ShouldEqual(new DateTime(2015, 12, 31));
        }

        [Test]
        public void Returns_null_when_value_is_null()
        {
            var retriever = new NullableDateTimeValueRetriever(new Mock<DateTimeValueRetriever>().Object);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_string_is_empty()
        {
            var retriever = new NullableDateTimeValueRetriever(new Mock<DateTimeValueRetriever>().Object);
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}