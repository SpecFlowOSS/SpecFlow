using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableByteValueRetrieverTests
    {
        [Test]
        public void Returns_null_when_the_value_is_null()
        {
            var retriever = new NullableByteValueRetriever(v => 0);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_ByteValueRetriever_when_passed_not_empty_string()
        {
            Func<string, byte> func = v =>
            {
                if (v == "test value") return 12;
                if (v == "another test value") return 34;
                return 0;
            };

            var retriever = new NullableByteValueRetriever(func);
            retriever.GetValue("test value").ShouldEqual<byte?>(12);
            retriever.GetValue("another test value").ShouldEqual<byte?>(34);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableByteValueRetriever(v => 3);
            retriever.GetValue(string.Empty).ShouldBeNull();
        }

    }
}