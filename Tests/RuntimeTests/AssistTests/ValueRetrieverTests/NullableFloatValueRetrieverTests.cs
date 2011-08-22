using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture, SetCulture("en-US")]
    public class NullableFloatValueRetrieverTests
    {
        [Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableFloatValueRetriever(v => 3.01F);
            retriever.GetValue(null).ShouldBeNull();
        }

        [Test]
        public void Returns_value_from_Single_value_retriever_when_not_empty()
        {
            Func<string, Single> func = v =>
                                            {
                                                if (v == "value 1") return 1F;
                                                if (v == "value 2") return 2F;
                                                return 0;
                                            };

            var retriever = new NullableFloatValueRetriever(func);
            retriever.GetValue("value 1").ShouldEqual(1F);
            retriever.GetValue("value 2").ShouldEqual(2F);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableFloatValueRetriever(v => 99F);
            retriever.GetValue("").ShouldBeNull();
        }
    }
}