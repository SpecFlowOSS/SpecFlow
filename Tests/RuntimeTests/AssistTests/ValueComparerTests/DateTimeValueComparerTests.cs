using System;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    [TestFixture]
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
            comparer.CompareValue("1/2/2011", DateTime.Parse("1/2/2011"))
                .ShouldBeTrue();
            comparer.CompareValue("12/31/2020", DateTime.Parse("12/31/2020"))
                .ShouldBeTrue();
        }

        [Test]
        public void Returns_false_when_the_string_and_values_match_for_different_dates()
        {
            var comparer = new DateTimeValueComparer();
            comparer.CompareValue("1/3/2011", DateTime.Parse("1/2/2011"))
                .ShouldBeFalse();
            comparer.CompareValue("1/2/2011", DateTime.Parse("1/3/2011"))
                .ShouldBeFalse();
            comparer.CompareValue("1/1/2011", DateTime.Parse("1/1/2012"))
                .ShouldBeFalse();
            comparer.CompareValue("1/1/2011", DateTime.Parse("2/1/2011"))
                .ShouldBeFalse();
        }

        [Test]
        public void Returns_false_when_the_expected_value_is_not_a_valid_datetime()
        {
            var comparer = new DateTimeValueComparer();
            comparer.CompareValue("x", DateTime.Parse("1/1/2020"))
                .ShouldBeFalse();
            comparer.CompareValue("January1", DateTime.Parse("1/1/2020"))
                .ShouldBeFalse();
        }

        [Test]
        public void Returns_false_the_value_is_correct_format_but_not_a_valid_date()
        {
            var comparer = new DateTimeValueComparer();
            comparer.CompareValue("2/29/2011", DateTime.Parse("2/28/2011"))
                .ShouldBeFalse();
            comparer.CompareValue("13/1/2011", DateTime.Parse("12/1/2011"))
                .ShouldBeFalse();
        }
    }
}