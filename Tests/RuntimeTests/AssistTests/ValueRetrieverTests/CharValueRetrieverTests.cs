using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class CharValueRetrieverTests
    {
        [Test]
        public void Returns_a_character_when_passed_a_character_value()
        {
            var retriever = new CharValueRetriever();
            retriever.GetValue("a").ShouldEqual('a');
            retriever.GetValue("A").ShouldEqual('A');
            retriever.GetValue("1").ShouldEqual('1');
            retriever.GetValue("&").ShouldEqual('&');
            retriever.GetValue(" ").ShouldEqual(' ');
        }

        [Test]
        public void Returns_char0_when_passed_empty()
        {
            var retriever = new CharValueRetriever();
            retriever.GetValue("").ShouldEqual('\0');
        }

        [Test]
        public void Returns_char0_when_passed_null()
        {
            var retriever = new CharValueRetriever();
            retriever.GetValue(null).ShouldEqual('\0');
        }

        [Test]
        public void Returns_char0_when_passed_multiple_characters()
        {
            var retriever = new CharValueRetriever();
            retriever.GetValue("ab").ShouldEqual('\0');
            retriever.GetValue("abc").ShouldEqual('\0');
            retriever.GetValue("abcdefg.").ShouldEqual('\0');
        }
    }
}