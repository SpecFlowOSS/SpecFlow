using System;
using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableBoolValueRetrieverTests
    {
        [Test]
        public void Returns_the_value_from_the_BoolValueRetriever()
        {
            var valueRetriever = new Mock<IValueRetriever<bool>>();
            valueRetriever.Setup(x => x.GetValue(It.IsAny<string>())).Returns(false);
            valueRetriever.Setup(x => x.GetValue("this value")).Returns(true);
            valueRetriever.Setup(x => x.GetValue("another value")).Returns(true);
            
            var retriever = new NullableBoolValueRetriever(valueRetriever.Object);
            retriever.GetValue("this value").ShouldEqual(true);
            retriever.GetValue("another value").ShouldEqual(true);
            retriever.GetValue("failing value").ShouldEqual(false);
            retriever.GetValue("another thing that returns false").ShouldEqual(false);
        }

        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableBoolValueRetriever(new BoolValueRetriever());
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableBoolValueRetriever(new BoolValueRetriever());
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}