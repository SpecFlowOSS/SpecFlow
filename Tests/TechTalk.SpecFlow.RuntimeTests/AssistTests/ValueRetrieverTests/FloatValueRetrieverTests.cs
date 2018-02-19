using FluentAssertions;
using NUnit.Framework;
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
            retriever.GetValue("0").Should().Be(0F);
            retriever.GetValue("1").Should().Be(1F);
            retriever.GetValue("2").Should().Be(2F);
            retriever.GetValue("2.23").Should().Be(2.23F);
            retriever.GetValue("384.234879").Should().Be(384.234879F);
		}

		[Test, SetCulture("fr-FR")]
		public void Returns_the_Float_value_when_passed_a_Float_string_if_culture_if_fr_FR()
		{
			var retriever = new FloatValueRetriever();
			retriever.GetValue("0").Should().Be(0F);
			retriever.GetValue("1").Should().Be(1F);
			retriever.GetValue("2").Should().Be(2F);
			retriever.GetValue("2,23").Should().Be(2.23F);
			retriever.GetValue("384,234879").Should().Be(384.234879F);
		}

		[Test]
        public void Returns_a_negative_Float_value_when_passed_one()
        {
            var retriever = new FloatValueRetriever();
            retriever.GetValue("-1").Should().Be(-1F);
            retriever.GetValue("-32.234").Should().Be(-32.234F);
        }

        [Test]
        public void Returns_zero_when_passed_a_non_numeric_value()
        {
            var retriever = new FloatValueRetriever();
            retriever.GetValue(null).Should().Be(0F);
            retriever.GetValue("").Should().Be(0F);
            retriever.GetValue("xxxslkdfj").Should().Be(0F);
        }
    }
}