using System;
using Moq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class NullableDoubleValueRetrieverTests : AssistTestsBase
    {
        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableDoubleValueRetriever();
            retriever.GetValue((string) null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_Double_value_retriever_when_not_empty()
        {
            var retriever = new NullableDoubleValueRetriever();
            retriever.GetValue("1").ShouldEqual(1);
            retriever.GetValue("2").ShouldEqual(2);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableDoubleValueRetriever();
            retriever.GetValue("").ShouldBeNull();
        }
    }
}