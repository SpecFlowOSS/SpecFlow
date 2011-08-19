using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class UShortValueRetrieverTests
    {
        [Test]
        public void Returns_an_unsigned_short_when_passed_an_unsigned_short_value()
        {
            var retriever = new UShortValueRetriever();
            retriever.GetValue("1").ShouldEqual<ushort>(1);
            retriever.GetValue("3").ShouldEqual<ushort>(3);
            retriever.GetValue("30").ShouldEqual<ushort>(30);
            retriever.GetValue("12345").ShouldEqual<ushort>(12345);
        }

        [Test]
        public void Returns_a_zero_when_passed_an_invalid_unsigned_short()
        {
            var retriever = new UShortValueRetriever();
            retriever.GetValue("x").ShouldEqual<ushort>(0);
            retriever.GetValue("-1").ShouldEqual<ushort>(0);
            retriever.GetValue("").ShouldEqual<ushort>(0);
            retriever.GetValue("1234567890").ShouldEqual<ushort>(0);
            retriever.GetValue("every good boy does fine").ShouldEqual<ushort>(0);
        }
    }
}