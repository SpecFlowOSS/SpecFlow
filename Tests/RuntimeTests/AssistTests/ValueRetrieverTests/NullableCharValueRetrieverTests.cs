using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableCharValueRetrieverTests
    {
        [Test]
        public void Returns_the_value_from_the_char_value_retriever()
        {
            var mock = new Mock<CharValueRetriever>();
            mock.Setup(x => x.GetValue("the first test value")).Returns('a');
            mock.Setup(x => x.GetValue("the second test value")).Returns('b');
            
            var retriever = new NullableCharValueRetriever(mock.Object);
            retriever.GetValue("the first test value").ShouldEqual('a');
            retriever.GetValue("the second test value").ShouldEqual('b');
        }

        [Test]
        public void Returns_null_when_value_is_empty()
        {
            var mock = new Mock<CharValueRetriever>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns('a');

            var retriever = new NullableCharValueRetriever(mock.Object);
            retriever.GetValue("").ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_value_is_null()
        {
            var mock = new Mock<CharValueRetriever>();
            mock.Setup(x => x.GetValue(It.IsAny<string>())).Returns('a');

            var retriever = new NullableCharValueRetriever(mock.Object);
            retriever.GetValue(null).ShouldBeNull();
        }
    }
}