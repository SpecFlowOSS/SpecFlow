using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class NullableDecimalValueRetrieverTests
    {
        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableDecimalValueRetriever(v => 23M);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_decimal_value_retriever_when_not_empty()
        {
            Func<string, decimal> func = v =>
                                             {
                                                 if (v == "value 1") return 1M;
                                                 if (v == "value 2") return 2M;
                                                 return 0M;
                                             };

            var retriever = new NullableDecimalValueRetriever(func);
            retriever.GetValue("value 1").ShouldEqual(1M);
            retriever.GetValue("value 2").ShouldEqual(2M);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableDecimalValueRetriever(v => 3M);
            retriever.GetValue("").ShouldBeNull();
        }
    }
}