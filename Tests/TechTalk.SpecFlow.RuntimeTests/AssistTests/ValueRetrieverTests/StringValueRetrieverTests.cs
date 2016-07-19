using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class StringValueRetrieverTests
    {
        [Test]
        public void Returns_the_string_value_back()
        {
            var retriever = new StringValueRetriever();
            retriever.GetValue("x").Should().Be("x");
            retriever.GetValue("X").Should().Be("X");
            retriever.GetValue("another value").Should().Be("another value");
        }
    }
}