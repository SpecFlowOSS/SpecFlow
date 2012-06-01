using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class NullableUShortValueRetrieverTests : AssistTestsBase
    {
        [Test]
        public void Returns_null_when_the_value_is_null()
        {
            var retriever = new NullableUShortValueRetriever();
            retriever.GetValue((string) null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_UShortValueRetriever_when_passed_not_empty_string()
        {
            var retriever = new NullableUShortValueRetriever();
            retriever.GetValue("123").ShouldEqual<ushort?>(123);
            retriever.GetValue("456").ShouldEqual<ushort?>(456);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableUShortValueRetriever();
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}