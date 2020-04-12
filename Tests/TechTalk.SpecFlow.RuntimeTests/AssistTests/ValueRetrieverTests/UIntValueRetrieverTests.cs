using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class UIntValueRetrieverTests
    {
		public UIntValueRetrieverTests()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
		}

        [Fact]
        public void Returns_an_unsigned_integer_when_passed_an_unsigned_integer_value()
        {
            var retriever = new UIntValueRetriever();
            retriever.GetValue("1").Should().Be(1);
            retriever.GetValue("3").Should().Be(3);
            retriever.GetValue("30").Should().Be(30);
            retriever.GetValue("1234567890").Should().Be(1234567890);
	        retriever.GetValue("1,234,567,890").Should().Be(1234567890);
		}

	    [Fact]
	    public void Returns_an_unsigned_integer_when_passed_an_unsigned_integer_value_if_fr_FR()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

			var retriever = new UIntValueRetriever();
		    retriever.GetValue("1").Should().Be(1);
		    retriever.GetValue("3").Should().Be(3);
		    retriever.GetValue("30").Should().Be(30);
		    retriever.GetValue("1234567890").Should().Be(1234567890);
	    }

        [Fact]
        public void Returns_a_zero_when_passed_an_invalid_unsigned_int()
        {
            var retriever = new UIntValueRetriever();
            retriever.GetValue("x").Should().Be(0);
            retriever.GetValue("-1").Should().Be(0);
            retriever.GetValue("").Should().Be(0);
            retriever.GetValue("every good boy does fine").Should().Be(0);
        }
    }
}