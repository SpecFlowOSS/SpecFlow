using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableULongValueRetrieverTests
    {
        [Test]
        public void Returns_null_when_the_value_is_null()
        {
            var retriever = new NullableULongValueRetriever(v => 0);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_ULongValueRetriever_when_passed_not_empty_string()
        {
            Func<string, ulong> func = v =>
            {
                if (v == "test value") return 123;
                if (v == "another test value") return 456;
                return 0;
            };

            var retriever = new NullableULongValueRetriever(func);
            retriever.GetValue("test value").ShouldEqual<ulong?>(123);
            retriever.GetValue("another test value").ShouldEqual<ulong?>(456);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableULongValueRetriever(v => 3);
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}