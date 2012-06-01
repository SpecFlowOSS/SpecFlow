using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class NullableULongValueRetrieverTests : AssistTestsBase
    {
        [Test]
        public void Returns_null_when_the_value_is_null()
        {
            var retriever = new NullableULongValueRetriever();
            retriever.GetValue((string) null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_ULongValueRetriever_when_passed_not_empty_string()
        {
            var retriever = new NullableULongValueRetriever();
            retriever.GetValue("123").ShouldEqual<ulong?>(123);
            retriever.GetValue("456").ShouldEqual<ulong?>(456);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableULongValueRetriever();
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}