using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class NullableFloatValueRetrieverTests : AssistTestsBase
    {
        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableFloatValueRetriever();
            retriever.GetValue((string) null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_Single_value_retriever_when_not_empty()
        {
            var retriever = new NullableFloatValueRetriever();
            retriever.GetValue("1").ShouldEqual(1F);
            retriever.GetValue("2").ShouldEqual(2F);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableFloatValueRetriever();
            retriever.GetValue("").ShouldBeNull();
        }
    }
}