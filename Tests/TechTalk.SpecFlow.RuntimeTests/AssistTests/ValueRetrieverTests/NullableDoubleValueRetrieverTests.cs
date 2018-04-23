﻿using System;
using System.Globalization;
using System.Threading;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class NullableDoubleValueRetrieverTests
	{
		[SetUp]
		public void TestSetup()
		{
			// this is required, because the tests depend on parsing decimals with the en-US culture
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		}

		[Test]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableDoubleValueRetriever(v => 3.01);
            retriever.GetValue(null).Should().Be(null);
        }

        [Test]
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

        [Test]
        public void Returns_null_when_passed_empty_string()
        {
            var retriever = new NullableDoubleValueRetriever(v => 99);
            retriever.GetValue("").Should().Be(null);
        }
    }
}