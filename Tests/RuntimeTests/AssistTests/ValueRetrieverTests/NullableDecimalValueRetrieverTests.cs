using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class NullableDecimalValueRetrieverTests : AssistTestsBase
    {
        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableDecimalValueRetriever();
            retriever.GetValue((string) null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_decimal_value_retriever_when_not_empty()
        {
            var retriever = new NullableDecimalValueRetriever();
            retriever.GetValue("1").ShouldEqual(1M);
            retriever.GetValue("2").ShouldEqual(2M);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableDecimalValueRetriever();
            retriever.GetValue("").ShouldBeNull();
        }
    }
}