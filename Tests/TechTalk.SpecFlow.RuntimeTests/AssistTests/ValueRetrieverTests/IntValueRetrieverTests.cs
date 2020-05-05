using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class IntValueRetrieverTests
    {
		public IntValueRetrieverTests()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
		}

        [Fact]
        public void Returns_an_integer_when_passed_an_integer_value()
        {
            var retriever = new IntValueRetriever();
            retriever.GetValue("1").Should().Be(1);
            retriever.GetValue("3").Should().Be(3);
            retriever.GetValue("30").Should().Be(30);
	        retriever.GetValue("1234567890").Should().Be(1234567890);
			retriever.GetValue("1,234,567,890").Should().Be(1234567890);
        }

        [Fact]
	    public void Returns_an_integer_when_passed_an_integer_value_if_culture_if_fr_FR()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

			var retriever = new IntValueRetriever();
		    retriever.GetValue("1").Should().Be(1);
		    retriever.GetValue("3").Should().Be(3);
		    retriever.GetValue("30").Should().Be(30);
		    retriever.GetValue("1234567890").Should().Be(1234567890);
		}

		[Fact]
        public void Returns_negative_numbers_when_passed_a_negative_value()
        {
            var retriever = new IntValueRetriever();
            retriever.GetValue("-1").Should().Be(-1);
            retriever.GetValue("-5").Should().Be(-5);
        }

        [Fact]
        public void Returns_a_zero_when_passed_an_invalid_int()
        {
            var retriever = new IntValueRetriever();
            retriever.GetValue("x").Should().Be(0);
            retriever.GetValue("").Should().Be(0);
            retriever.GetValue("every good boy does fine").Should().Be(0);
		}

	    [Fact]
	    public void Returns_a_zero_when_passed_an_invalid_int_if_culture_is_fr_FR()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

			var retriever = new IntValueRetriever();
		    retriever.GetValue("1,234,567,890").Should().Be(0);
		}
	}
}