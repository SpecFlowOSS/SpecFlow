using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableBoolValueRetrieverTests
    {
        [Test]
        public void Returns_the_value_from_the_BoolValueRetriever()
        {
            Func<string, bool> func = value => value == "this value" || value == "another value";
            
            var retriever = new NullableBoolValueRetriever(func);
            retriever.GetValue("this value").ShouldEqual(true);
            retriever.GetValue("another value").ShouldEqual(true);
            retriever.GetValue("failing value").ShouldEqual(false);
            retriever.GetValue("another thing that returns false").ShouldEqual(false);
        }

        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableBoolValueRetriever(value => true);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableBoolValueRetriever(value => true);
            retriever.GetValue(string.Empty).ShouldBeNull();
        }
    }
}