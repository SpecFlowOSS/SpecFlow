using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    [TestFixture, SetCulture("en-US")]
    public class DateTimeValueComparerTests
    {
        [Test]
        public void Can_compare_if_the_value_is_a_datetime()
        {
            new DateTimeValueComparer()
                .CanCompare(new DateTime())
                .ShouldBeTrue();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_not_a_datetime()
        {
            var comparer = new DateTimeValueComparer();
            comparer.CanCompare(3).ShouldBeFalse();
            comparer.CanCompare("x").ShouldBeFalse();
            comparer.CanCompare(4.3).ShouldBeFalse();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_null()
        {
            new DateTimeValueComparer()
                .CanCompare(null)
                .ShouldBeFalse();
        }

        [Test]
        public void Returns_true_when_the_string_and_values_match_exactly()
        {
            var comparer = new DateTimeValueComparer();
            comparer.TheseValuesAreTheSame(new DateTime(2011, 1, 2).ToString(), new DateTime(2011, 1, 2))
                .ShouldBeTrue();
            comparer.TheseValuesAreTheSame(new DateTime(2020, 12, 31).ToString(), new DateTime(2020, 12, 31))
                .ShouldBeTrue();
        }

        [Test]
        public void Returns_false_when_the_string_and_values_match_for_different_dates()
        {
            var comparer = new DateTimeValueComparer();
            comparer.TheseValuesAreTheSame(new DateTime(2011, 1, 3).ToString(), new DateTime(2011, 1, 2))
                .ShouldBeFalse();
            comparer.TheseValuesAreTheSame(new DateTime(2011, 1, 2).ToString(), new DateTime(2011, 1, 3))
                .ShouldBeFalse();
            comparer.TheseValuesAreTheSame(new DateTime(2011, 1, 1).ToString(), new DateTime(2012, 1, 1))
                .ShouldBeFalse();
            comparer.TheseValuesAreTheSame(new DateTime(2011, 1, 1).ToString(), new DateTime(2011, 2, 1))
                .ShouldBeFalse();
        }

        [Test]
        public void Returns_false_when_the_expected_value_is_not_a_valid_datetime()
        {
            var comparer = new DateTimeValueComparer();
            comparer.TheseValuesAreTheSame("x", new DateTime(2020, 1, 1))
                .ShouldBeFalse();
            comparer.TheseValuesAreTheSame("January1", new DateTime(2020, 1, 1))
                .ShouldBeFalse();
        }

        [Test]
        public void Returns_false_the_value_is_correct_format_but_not_a_valid_date()
        {
            var comparer = new DateTimeValueComparer();
            comparer.TheseValuesAreTheSame(new DateTime(2011, 2, 28).ToString().Replace("28", "29"), new DateTime(2011, 2, 28))
                .ShouldBeFalse();
            comparer.TheseValuesAreTheSame(new DateTime(2011, 12, 1).ToString().Replace("12", "13"), new DateTime(2011, 12, 1))
                .ShouldBeFalse();
        }
    }
}
