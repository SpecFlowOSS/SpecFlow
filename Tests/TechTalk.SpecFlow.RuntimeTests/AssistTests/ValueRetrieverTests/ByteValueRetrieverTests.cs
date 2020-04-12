using FluentAssertions;
using Xunit;
using System.Globalization;
using System.Threading;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class ByteValueRetrieverTests
    {
        [Fact]
        public void Returns_a_byte_when_passed_a_byte_value()
        {
            var retriever = new ByteValueRetriever();
            retriever.GetValue("1").Should().Be(1);
            retriever.GetValue("3").Should().Be(3);
            retriever.GetValue("30").Should().Be(30);
		}

        [Fact]
	    public void Returns_a_byte_when_passed_a_byte_value_if_culture_is_fr_Fr()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR", false);

			var retriever = new ByteValueRetriever();
		    retriever.GetValue("30,0").Should().Be(30);
	    }

		[Fact]
        public void Returns_a_zero_when_passed_an_invalid_byte()
        {
            var retriever = new ByteValueRetriever();
            retriever.GetValue("x").Should().Be(0);
            retriever.GetValue("").Should().Be(0);
            retriever.GetValue("-1").Should().Be(0);
            retriever.GetValue("500").Should().Be(0);
            retriever.GetValue("every good boy does fine").Should().Be(0);
        }
	}
}