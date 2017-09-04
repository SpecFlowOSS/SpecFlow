using System;
using System.Globalization;
using System.Threading;
using Moq;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    public class NullableDoubleValueRetrieverTests
    {
        public NullableDoubleValueRetrieverTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        [Fact]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableDoubleValueRetriever(v => 3.01);
            retriever.GetValue(null).Should().Be(null);
        }

        [Fact]
        public void Returns_value_from_Double_value_retriever_when_not_empty()
        {
            var mock = new Mock<DoubleValueRetriever>();
            mock.Setup(x => x.GetValue("value 1")).Returns(1);
            mock.Setup(x => x.GetValue("value 2")).Returns(2);
            Func<string, double> func = v =>
                                            {
                                                if (v == "value 1") return 1;
                                                if (v == "value 2") return 2;
                                                return 0;
                                            };

            var retriever = new NullableDoubleValueRetriever(func);
            retriever.GetValue("value 1").Should().Be(1);
            retriever.GetValue("value 2").Should().Be(2);
        }

        [Fact]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableDoubleValueRetriever(v => 99);
            retriever.GetValue("").Should().Be(null);
        }
    }
}