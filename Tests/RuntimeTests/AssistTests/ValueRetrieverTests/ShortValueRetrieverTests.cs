using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class ShortValueRetrieverTests
    {
        [Test]
        public void Returns_a_short_when_passed_a_short_value()
        {
            var retriever = new ShortValueRetriever();
            retriever.GetValue("1").ShouldEqual<short>(1);
            retriever.GetValue("3").ShouldEqual<short>(3);
            retriever.GetValue("30").ShouldEqual<short>(30);
            retriever.GetValue("12345").ShouldEqual<short>(12345);
        }

        [Test]
        public void Returns_negative_numbers_when_passed_a_negative_value()
        {
            var retriever = new ShortValueRetriever();
            retriever.GetValue("-1").ShouldEqual<short>(-1);
            retriever.GetValue("-5").ShouldEqual<short>(-5);
        }

        [Test]
        public void Returns_a_zero_when_passed_an_invalid_short()
        {
            var retriever = new IntValueRetriever();
            retriever.GetValue("x").ShouldEqual(0);
            retriever.GetValue("").ShouldEqual(0);
            retriever.GetValue("1234567890123456789").ShouldEqual(0);
            retriever.GetValue("every good boy does fine").ShouldEqual(0);
        }
    }
}