﻿using System.Globalization;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class DecimalValueRetreiverTests
	{
		[SetUp]
		public void TestSetup()
		{
			// this is required, because the tests depend on parsing decimals with the en-US culture
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		}

		[Test]
        public void Returns_the_decimal_value_when_passed_a_decimal_string()
        {
            var retriever = new DecimalValueRetriever();
            retriever.GetValue("0").Should().Be(0M);
            retriever.GetValue("1").Should().Be(1M);
            retriever.GetValue("2").Should().Be(2M);
            retriever.GetValue("2.23").Should().Be(2.23M);
            retriever.GetValue("384.234879").Should().Be(384.234879M);
		}

	    [Test]
	    public void Returns_the_decimal_value_when_passed_a_decimal_string_if_culture_if_fr_FR()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

			var retriever = new DecimalValueRetriever();
		    retriever.GetValue("0").Should().Be(0M);
		    retriever.GetValue("1").Should().Be(1M);
		    retriever.GetValue("2").Should().Be(2M);
		    retriever.GetValue("2,23").Should().Be(2.23M);
			retriever.GetValue("384,234879").Should().Be(384.234879M);
	    }

		[Test]
        public void Returns_a_negative_decimal_value_when_passed_one()
        {
            var retriever = new DecimalValueRetriever();
            retriever.GetValue("-1").Should().Be(-1M);
            retriever.GetValue("-32.234").Should().Be(-32.234M);
        }

        [Test]
        public void Returns_zero_when_passed_a_non_numeric_value()
        {
            var retriever = new DecimalValueRetriever();
            retriever.GetValue(null).Should().Be(0M);
            retriever.GetValue("").Should().Be(0M);
            retriever.GetValue("xxxslkdfj").Should().Be(0M);
        }
    }
}