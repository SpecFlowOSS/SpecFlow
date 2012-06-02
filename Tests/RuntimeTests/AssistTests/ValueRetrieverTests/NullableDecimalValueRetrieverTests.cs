using System;
using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class NullableDecimalValueRetrieverTests
    {
        [Test]
        public void Returns_null_when_passed_null()
        {
            var mock = new Mock<IValueRetriever<decimal>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(23M);

            var retriever = new NullableDecimalValueRetriever(mock.Object);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_decimal_value_retriever_when_not_empty()
        {
            var mock = new Mock<IValueRetriever<decimal>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(0M);
            mock.Setup(x => x.GetValue("value 1")).Returns(1M);
            mock.Setup(x => x.GetValue("value 2")).Returns(2M);

            var retriever = new NullableDecimalValueRetriever(mock.Object);
            retriever.GetValue("value 1").ShouldEqual(1M);
            retriever.GetValue("value 2").ShouldEqual(2M);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var mock = new Mock<IValueRetriever<decimal>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(3M);

            var retriever = new NullableDecimalValueRetriever(mock.Object);
            retriever.GetValue("").ShouldBeNull();
        }
    }
}