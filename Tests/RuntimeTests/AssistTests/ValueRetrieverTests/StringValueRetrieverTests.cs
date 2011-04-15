using NUnit.Framework;
using Should;
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
            retriever.GetValue("x").ShouldEqual("x");
            retriever.GetValue("X").ShouldEqual("X");
            retriever.GetValue("another value").ShouldEqual("another value");
        }
    }
}