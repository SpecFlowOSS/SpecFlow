using System;
using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class NullableFloatValueRetrieverTests
    {
        [Test]
        public void Returns_null_when_passed_null()
        {
            var mock = new Mock<IValueRetriever<float>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(3.01F);

            var retriever = new NullableFloatValueRetriever(mock.Object);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_Single_value_retriever_when_not_empty()
        {
            var mock = new Mock<IValueRetriever<float>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(0);
            mock.Setup(x => x.GetValue("value 1")).Returns(1F);
            mock.Setup(x => x.GetValue("value 2")).Returns(2F);

            var retriever = new NullableFloatValueRetriever(mock.Object);
            retriever.GetValue("value 1").ShouldEqual(1F);
            retriever.GetValue("value 2").ShouldEqual(2F);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var mock = new Mock<IValueRetriever<float>>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns(99F);

            var retriever = new NullableFloatValueRetriever(mock.Object);
            retriever.GetValue("").ShouldBeNull();
        }
    }
}