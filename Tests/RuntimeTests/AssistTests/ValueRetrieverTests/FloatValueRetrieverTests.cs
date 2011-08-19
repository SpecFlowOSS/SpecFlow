using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class FloatValueRetrieverTests
    {
        [Test]
        public void Returns_the_Float_value_when_passed_a_Float_string()
        {
            var retriever = new FloatValueRetriever();
            retriever.GetValue("0").ShouldEqual(0F);
            retriever.GetValue("1").ShouldEqual(1F);
            retriever.GetValue("2").ShouldEqual(2F);
            retriever.GetValue("2.23").ShouldEqual(2.23F);
            retriever.GetValue("384.234879").ShouldEqual(384.234879F);
        }

        [Test]
        public void Returns_a_negative_Float_value_when_passed_one()
        {
            var retriever = new FloatValueRetriever();
            retriever.GetValue("-1").ShouldEqual(-1F);
            retriever.GetValue("-32.234").ShouldEqual(-32.234F);
        }

        [Test]
        public void Returns_zero_when_passed_a_non_numeric_value()
        {
            var retriever = new FloatValueRetriever();
            retriever.GetValue(null).ShouldEqual(0F);
            retriever.GetValue("").ShouldEqual(0F);
            retriever.GetValue("xxxslkdfj").ShouldEqual(0F);
        }
    }
}