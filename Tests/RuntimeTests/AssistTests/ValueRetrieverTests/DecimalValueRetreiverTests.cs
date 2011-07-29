using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class DecimalValueRetreiverTests
    {
        [Test]
        public void Returns_the_decimal_value_when_passed_a_decimal_string()
        {
            var retriever = new DecimalValueRetriever();
            retriever.GetValue("0").ShouldEqual(0M);
            retriever.GetValue("1").ShouldEqual(1M);
            retriever.GetValue("2").ShouldEqual(2M);
            retriever.GetValue("2.23").ShouldEqual(2.23M);
            retriever.GetValue("384.234879").ShouldEqual(384.234879M);
        }

        [Test]
        public void Returns_a_negative_decimal_value_when_passed_one()
        {
            var retriever = new DecimalValueRetriever();
            retriever.GetValue("-1").ShouldEqual(-1M);
            retriever.GetValue("-32.234").ShouldEqual(-32.234M);
        }

        [Test]
        public void Returns_zero_when_passed_a_non_numeric_value()
        {
            var retriever = new DecimalValueRetriever();
            retriever.GetValue(null).ShouldEqual(0M);
            retriever.GetValue("").ShouldEqual(0M);
            retriever.GetValue("xxxslkdfj").ShouldEqual(0M);
        }
    }
}