using System;
using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class NullableUIntValueRetrieverTests
    {
        [Test]
        public void Returns_null_when_the_value_is_null()
        {
            var mock = new Mock<IValueRetriever<uint>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(0);

            var retriever = new NullableUIntValueRetriever(mock.Object);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_UIntValueRetriever_when_passed_not_empty_string()
        {
            var mock = new Mock<IValueRetriever<uint>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(0);
            mock.Setup(x => x.GetValue("test value")).Returns(123);
            mock.Setup(x => x.GetValue("another test value")).Returns(456);

            var retriever = new NullableUIntValueRetriever(mock.Object);
            retriever.GetValue("test value").ShouldEqual<uint?>(123);
            retriever.GetValue("another test value").ShouldEqual<uint?>(456);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var mock = new Mock<IValueRetriever<uint>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(3);

            var retriever = new NullableUIntValueRetriever(mock.Object);
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}