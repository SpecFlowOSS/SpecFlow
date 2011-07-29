using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class DoubleValueRetreiverTests
    {
        [Test]
        public void Returns_the_Double_value_when_passed_a_Double_string()
        {
            var retriever = new DoubleValueRetriever();
            retriever.GetValue("0").ShouldEqual(0);
            retriever.GetValue("1").ShouldEqual(1);
            retriever.GetValue("2").ShouldEqual(2);
            retriever.GetValue("2.23").ShouldEqual(2.23);
            retriever.GetValue("384.234879").ShouldEqual(384.234879);
        }

        [Test]
        public void Returns_a_negative_Double_value_when_passed_one()
        {
            var retriever = new DoubleValueRetriever();
            retriever.GetValue("-1").ShouldEqual(-1);
            retriever.GetValue("-32.234").ShouldEqual(-32.234);
        }

        [Test]
        public void Returns_zero_when_passed_a_non_numeric_value()
        {
            var retriever = new DoubleValueRetriever();
            retriever.GetValue(null).ShouldEqual(0);
            retriever.GetValue("").ShouldEqual(0);
            retriever.GetValue("xxxslkdfj").ShouldEqual(0);
        }
    }
}