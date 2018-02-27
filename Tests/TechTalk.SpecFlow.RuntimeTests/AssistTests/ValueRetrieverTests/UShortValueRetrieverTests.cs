﻿using System.Globalization;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    [TestFixture]
    public class UShortValueRetrieverTests
	{
		[SetUp]
		public void TestSetup()
		{
			// this is required, because the tests depend on parsing decimals with the en-US culture
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
		}

		[Test]
        public void Returns_an_unsigned_short_when_passed_an_unsigned_short_value()
        {
            var retriever = new UShortValueRetriever();
            retriever.GetValue("1").Should().Be(1);
            retriever.GetValue("3").Should().Be(3);
            retriever.GetValue("30").Should().Be(30);
            retriever.GetValue("12345").Should().Be(12345);
	        retriever.GetValue("12,345").Should().Be(12345);
		}

	    [Test]
	    public void Returns_an_unsigned_short_when_passed_an_unsigned_short_value_if_culture_is_fr_FR()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

			var retriever = new UShortValueRetriever();
		    retriever.GetValue("1").Should().Be(1);
		    retriever.GetValue("3").Should().Be(3);
		    retriever.GetValue("30").Should().Be(30);
		    retriever.GetValue("12345").Should().Be(12345);
	    }

		[Test]
        public void Returns_a_zero_when_passed_an_invalid_unsigned_short()
        {
            var retriever = new UShortValueRetriever();
            retriever.GetValue("x").Should().Be(0);
            retriever.GetValue("-1").Should().Be(0);
            retriever.GetValue("").Should().Be(0);
            retriever.GetValue("1234567890").Should().Be(0);
            retriever.GetValue("every good boy does fine").Should().Be(0);
        }
    }
}