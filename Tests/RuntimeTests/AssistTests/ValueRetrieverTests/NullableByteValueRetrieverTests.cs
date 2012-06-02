using System;
using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableByteValueRetrieverTests
    {
        [Test]
        public void Returns_null_when_the_value_is_null()
        {
            var retriever = new NullableByteValueRetriever(new ByteValueRetriever());
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_ByteValueRetriever_when_passed_not_empty_string()
        {
            var valueRetrieverMock = new Mock<IValueRetriever<byte>>();
            valueRetrieverMock.Setup(x => x.GetValue("test value")).Returns(12);
            valueRetrieverMock.Setup(x => x.GetValue("another test value")).Returns(34);

            var retriever = new NullableByteValueRetriever(valueRetrieverMock.Object);
            retriever.GetValue("test value").ShouldEqual<byte?>(12);
            retriever.GetValue("another test value").ShouldEqual<byte?>(34);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableByteValueRetriever(new ByteValueRetriever());
            retriever.GetValue(string.Empty).ShouldBeNull();
        }

    }
}