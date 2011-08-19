using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class ByteValueRetrieverTests
    {
        [Test]
        public void Returns_a_byte_when_passed_a_byte_value()
        {
            var retriever = new ByteValueRetriever();
            retriever.GetValue("1").ShouldEqual<byte>(1);
            retriever.GetValue("3").ShouldEqual<byte>(3);
            retriever.GetValue("30").ShouldEqual<byte>(30);
        }

        [Test]
        public void Returns_a_zero_when_passed_an_invalid_byte()
        {
            var retriever = new ByteValueRetriever();
            retriever.GetValue("x").ShouldEqual<byte>(0);
            retriever.GetValue("").ShouldEqual<byte>(0);
            retriever.GetValue("-1").ShouldEqual<byte>(0);
            retriever.GetValue("500").ShouldEqual<byte>(0);
            retriever.GetValue("every good boy does fine").ShouldEqual<byte>(0);
        }
    }
}