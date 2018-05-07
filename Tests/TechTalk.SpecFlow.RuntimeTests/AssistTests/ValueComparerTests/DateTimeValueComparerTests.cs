﻿using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    [TestFixture]
    public class DateTimeValueComparerTests
    {
	    [SetUp]
	    public void TestSetup()
	    {
		    // this is required, because the tests depend on parsing decimals with the en-US culture
		    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
	    }

		[Test]
        public void Can_compare_if_the_value_is_a_datetime()
        {
            new DateTimeValueComparer()
                .CanCompare(new DateTime())
                .Should().BeTrue();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_not_a_datetime()
        {
            var comparer = new DateTimeValueComparer();
            comparer.CanCompare(3).Should().BeFalse();
            comparer.CanCompare("x").Should().BeFalse();
            comparer.CanCompare(4.3).Should().BeFalse();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_null()
        {
            new DateTimeValueComparer()
                .CanCompare(null)
                .Should().BeFalse();
        }

        [Test]
        public void Returns_true_when_the_string_and_values_match_exactly()
        {
            var comparer = new DateTimeValueComparer();
            comparer.Compare(new DateTime(2011, 1, 2).ToString(), new DateTime(2011, 1, 2))
                .Should().BeTrue();
            comparer.Compare(new DateTime(2020, 12, 31).ToString(), new DateTime(2020, 12, 31))
                .Should().BeTrue();
        }

        [Test]
        public void Returns_false_when_the_string_and_values_match_for_different_dates()
        {
            var comparer = new DateTimeValueComparer();
            comparer.Compare(new DateTime(2011, 1, 3).ToString(), new DateTime(2011, 1, 2))
                .Should().BeFalse();
            comparer.Compare(new DateTime(2011, 1, 2).ToString(), new DateTime(2011, 1, 3))
                .Should().BeFalse();
            comparer.Compare(new DateTime(2011, 1, 1).ToString(), new DateTime(2012, 1, 1))
                .Should().BeFalse();
            comparer.Compare(new DateTime(2011, 1, 1).ToString(), new DateTime(2011, 2, 1))
                .Should().BeFalse();
        }

        [Test]
        public void Returns_false_when_the_expected_value_is_not_a_valid_datetime()
        {
            var comparer = new DateTimeValueComparer();
            comparer.Compare("x", new DateTime(2020, 1, 1))
                .Should().BeFalse();
            comparer.Compare("January1", new DateTime(2020, 1, 1))
                .Should().BeFalse();
        }

        [Test]
        public void Returns_false_the_value_is_correct_format_but_not_a_valid_date()
        {
            var comparer = new DateTimeValueComparer();
            comparer.Compare(new DateTime(2011, 2, 28).ToString().Replace("28", "29"), new DateTime(2011, 2, 28))
                .Should().BeFalse();
            comparer.Compare(new DateTime(2011, 12, 1).ToString().Replace("12", "13"), new DateTime(2011, 12, 1))
                .Should().BeFalse();
        }
    }
}
