using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class SByteValueRetrieverTests
    {
        [Test]
        public void Returns_a_signed_byte_when_passed_a_signed_byte_value()
        {
            var retriever = new SByteValueRetriever();
            retriever.GetValue("1").ShouldEqual<sbyte>(1);
            retriever.GetValue("3").ShouldEqual<sbyte>(3);
            retriever.GetValue("30").ShouldEqual<sbyte>(30);
        }

        [Test]
        public void Returns_negative_numbers_when_passed_a_negative_value()
        {
            var retriever = new SByteValueRetriever();
            retriever.GetValue("-1").ShouldEqual<sbyte>(-1);
            retriever.GetValue("-5").ShouldEqual<sbyte>(-5);
        }

        [Test]
        public void Returns_a_zero_when_passed_an_invalid_signed_byte()
        {
            var retriever = new SByteValueRetriever();
            retriever.GetValue("x").ShouldEqual<sbyte>(0);
            retriever.GetValue("").ShouldEqual<sbyte>(0);
            retriever.GetValue("500").ShouldEqual<sbyte>(0);
            retriever.GetValue("every good boy does fine").ShouldEqual<sbyte>(0);
        }
    }
}