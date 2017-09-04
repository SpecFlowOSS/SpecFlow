using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class CharValueRetrieverTests
    {
        [Fact]
        public void Returns_a_character_when_passed_a_character_value()
        {
            var retriever = new CharValueRetriever();
            retriever.GetValue("a").Should().Be('a');
            retriever.GetValue("A").Should().Be('A');
            retriever.GetValue("1").Should().Be('1');
            retriever.GetValue("&").Should().Be('&');
            retriever.GetValue(" ").Should().Be(' ');
        }

        [Fact]
        public void Returns_char0_when_passed_empty()
        {
            var retriever = new CharValueRetriever();
            retriever.GetValue("").Should().Be('\0');
        }

        [Fact]
        public void Returns_char0_when_passed_null()
        {
            var retriever = new CharValueRetriever();
            retriever.GetValue(null).Should().Be('\0');
        }

        [Fact]
        public void Returns_char0_when_passed_multiple_characters()
        {
            var retriever = new CharValueRetriever();
            retriever.GetValue("ab").Should().Be('\0');
            retriever.GetValue("abc").Should().Be('\0');
            retriever.GetValue("abcdefg.").Should().Be('\0');
        }
    }
}