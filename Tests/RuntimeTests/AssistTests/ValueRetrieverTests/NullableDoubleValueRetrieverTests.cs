using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableDoubleValueRetrieverTests
    {
        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableDoubleValueRetriever(new Mock<DoubleValueRetriever>().Object);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_Double_value_retriever_when_not_empty()
        {
            var mock = new Mock<DoubleValueRetriever>();
            mock.Setup(x => x.GetValue("value 1")).Returns(1);
            mock.Setup(x => x.GetValue("value 2")).Returns(2);

            var retriever = new NullableDoubleValueRetriever(mock.Object);
            retriever.GetValue("value 1").ShouldEqual(1);
            retriever.GetValue("value 2").ShouldEqual(2);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableDoubleValueRetriever(new Mock<DoubleValueRetriever>().Object);
            retriever.GetValue("").ShouldBeNull();
        }
    }
}