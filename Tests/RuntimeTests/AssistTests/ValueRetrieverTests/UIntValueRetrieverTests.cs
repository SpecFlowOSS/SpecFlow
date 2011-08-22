using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class UIntValueRetrieverTests
    {
        [Test]
        public void Returns_an_unsigned_integer_when_passed_an_unsigned_integer_value()
        {
            var retriever = new UIntValueRetriever();
            retriever.GetValue("1").ShouldEqual<uint>(1);
            retriever.GetValue("3").ShouldEqual<uint>(3);
            retriever.GetValue("30").ShouldEqual<uint>(30);
            retriever.GetValue("1234567890").ShouldEqual<uint>(1234567890);
        }

        [Test]
        public void Returns_a_zero_when_passed_an_invalid_unsigned_int()
        {
            var retriever = new UIntValueRetriever();
            retriever.GetValue("x").ShouldEqual<uint>(0);
            retriever.GetValue("-1").ShouldEqual<uint>(0);
            retriever.GetValue("").ShouldEqual<uint>(0);
            retriever.GetValue("every good boy does fine").ShouldEqual<uint>(0);
        }
    }
}