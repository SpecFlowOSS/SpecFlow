using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class ULongValueRetrieverTests
    {
        [Fact]
        public void Returns_an_unsigned_long_when_passed_an_unsigned_long_value()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            
            var retriever = new ULongValueRetriever();

            retriever.GetValue("1").Should().Be(1);
            retriever.GetValue("3").Should().Be(3);
            retriever.GetValue("30").Should().Be(30);
            retriever.GetValue("12345678901234567890").Should().Be(12345678901234567890);
	        retriever.GetValue("12,345,678,901,234,567,890").Should().Be(12345678901234567890);
		}

        [Fact]
        public void Returns_a_zero_when_passed_an_invalid_unsigned_long()
        {
            var retriever = new ULongValueRetriever();
            retriever.GetValue("x").Should().Be(0);
            retriever.GetValue("-1").Should().Be(0);
            retriever.GetValue("").Should().Be(0);
            retriever.GetValue("every good boy does fine").Should().Be(0);
        }
    }
}