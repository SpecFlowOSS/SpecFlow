using System;
using NUnit.Framework;
using FluentAssertions;
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
            retriever.GetValue(null).Should().Be(null);
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
            retriever.GetValue("value 1").Should().Be(1F);
            retriever.GetValue("value 2").Should().Be(2F);
        }

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableFloatValueRetriever(v => 99F);
            retriever.GetValue("").Should().Be(null);
        }
    }
}