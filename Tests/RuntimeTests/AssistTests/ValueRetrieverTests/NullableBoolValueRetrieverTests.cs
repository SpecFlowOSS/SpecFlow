using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableBoolValueRetrieverTests
    {
        [Test]
        public void Returns_the_value_from_the_BoolValueRetriever()
        {
            var mock = new Mock<BoolValueRetriever>();
            mock.Setup(x => x.GetValue("this value")).Returns(true);
            mock.Setup(x => x.GetValue("another value")).Returns(true);
            mock.Setup(x => x.GetValue("failing value")).Returns(false);
            mock.Setup(x => x.GetValue("another thing that returns false")).Returns(false);
            
            var retriever = new NullableBoolValueRetriever(mock.Object);
            retriever.GetValue("this value").ShouldEqual(true);
            retriever.GetValue("another value").ShouldEqual(true);
            retriever.GetValue("failing value").ShouldEqual(false);
            retriever.GetValue("another thing that returns false").ShouldEqual(false);
        }

        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableBoolValueRetriever(new Mock<BoolValueRetriever>().Object);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableBoolValueRetriever(new Mock<BoolValueRetriever>().Object);
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}