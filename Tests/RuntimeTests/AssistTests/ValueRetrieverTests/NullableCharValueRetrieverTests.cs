using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableCharValueRetrieverTests : AssistTestsBase
    {
        [Test]
        public void Returns_the_value_from_the_char_value_retriever()
        {
            var retriever = new NullableCharValueRetriever();
            retriever.GetValue("a").ShouldEqual('a');
            retriever.GetValue("b").ShouldEqual('b');
        }

        [Test]
        public void Returns_null_when_value_is_empty()
        {
            var retriever = new NullableCharValueRetriever();
            retriever.GetValue("").ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_value_is_null()
        {
            var retriever = new NullableCharValueRetriever();
            retriever.GetValue((string) null).ShouldBeNull();
        }
    }
}