using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class ULongValueRetrieverTests
    {
        [Test]
        public void Returns_an_unsigned_long_when_passed_an_unsigned_long_value()
        {
            var retriever = new ULongValueRetriever();
            retriever.GetValue("1").ShouldEqual<ulong>(1);
            retriever.GetValue("3").ShouldEqual<ulong>(3);
            retriever.GetValue("30").ShouldEqual<ulong>(30);
            retriever.GetValue("12345678901234567890").ShouldEqual<ulong>(12345678901234567890);
        }

        [Test]
        public void Returns_a_zero_when_passed_an_invalid_unsigned_long()
        {
            var retriever = new ULongValueRetriever();
            retriever.GetValue("x").ShouldEqual<ulong>(0);
            retriever.GetValue("-1").ShouldEqual<ulong>(0);
            retriever.GetValue("").ShouldEqual<ulong>(0);
            retriever.GetValue("every good boy does fine").ShouldEqual<ulong>(0);
        }
    }
}