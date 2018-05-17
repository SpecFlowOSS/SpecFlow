using System.Globalization;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class DoubleValueRetreiverTests
	{
		[SetUp]
		public void TestSetup()
		{
			// this is required, because the tests depend on parsing decimals with the en-US culture
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		}

		[Test]
        public void Returns_the_Double_value_when_passed_a_Double_string()
        {
            var retriever = new DoubleValueRetriever();
            retriever.GetValue("0").Should().Be(0);
            retriever.GetValue("1").Should().Be(1);
            retriever.GetValue("2").Should().Be(2);
            retriever.GetValue("2.23").Should().Be(2.23);
            retriever.GetValue("384.234879").Should().Be(384.234879);
        }

		[Test]
		public void Returns_the_Double_value_when_passed_a_Double_string_If_Culture_Is_fr_Fr()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

			var retriever = new DoubleValueRetriever();
			retriever.GetValue("0").Should().Be(0);
			retriever.GetValue("1").Should().Be(1);
			retriever.GetValue("2").Should().Be(2);
			retriever.GetValue("2,23").Should().Be(2.23);
			retriever.GetValue("384,234879").Should().Be(384.234879);
		}

		[Test]
        public void Returns_a_negative_Double_value_when_passed_one()
        {
            var retriever = new DoubleValueRetriever();
            retriever.GetValue("-1").Should().Be(-1);
            retriever.GetValue("-32.234").Should().Be(-32.234);
        }

        [Test]
        public void Returns_zero_when_passed_a_non_numeric_value()
        {
            var retriever = new DoubleValueRetriever();
            retriever.GetValue(null).Should().Be(0);
            retriever.GetValue("").Should().Be(0);
            retriever.GetValue("xxxslkdfj").Should().Be(0);
        }
    }
}