using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class NullableByteValueRetrieverTests : AssistTestsBase
    {
        [Test]
        public void Returns_null_when_the_value_is_null()
        {
            var retriever = new NullableByteValueRetriever();
            retriever.GetValue((string) null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_ByteValueRetriever_when_passed_not_empty_string()
        {
            var retriever = new NullableByteValueRetriever();
            retriever.GetValue("12").ShouldEqual<byte?>(12);
            retriever.GetValue("34").ShouldEqual<byte?>(34);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableByteValueRetriever();
            retriever.GetValue(string.Empty).ShouldBeNull();
        }

    }
}