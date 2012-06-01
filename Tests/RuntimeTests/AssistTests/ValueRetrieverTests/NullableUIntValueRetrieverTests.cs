using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class NullableUIntValueRetrieverTests : AssistTestsBase
    {
        [Test]
        public void Returns_null_when_the_value_is_null()
        {
            var retriever = new NullableUIntValueRetriever();
            retriever.GetValue((string) null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_UIntValueRetriever_when_passed_not_empty_string()
        {
            var retriever = new NullableUIntValueRetriever();
            retriever.GetValue("123").ShouldEqual<uint?>(123);
            retriever.GetValue("456").ShouldEqual<uint?>(456);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableUIntValueRetriever();
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}