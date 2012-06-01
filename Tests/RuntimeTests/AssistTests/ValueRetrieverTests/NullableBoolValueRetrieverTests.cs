using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableBoolValueRetrieverTests : AssistTestsBase
    {
        [Test]
        public void Returns_the_value_from_the_BoolValueRetriever()
        {
            var retriever = new NullableBoolValueRetriever();
            retriever.GetValue("true").ShouldEqual(true);
            retriever.GetValue("false").ShouldEqual(false);
        }

        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableBoolValueRetriever();
            retriever.GetValue((string) null).ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableBoolValueRetriever();
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}