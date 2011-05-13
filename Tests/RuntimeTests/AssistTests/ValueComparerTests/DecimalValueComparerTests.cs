using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    [TestFixture, SetCulture("en-US")]
    public class DecimalValueComparerTests
    {
        [Test]
        public void Can_compare_if_the_value_is_a_decimal()
        {
            var valueComparer = new DecimalValueComparer();
            valueComparer.CanCompare(1.0M).ShouldBeTrue();
            valueComparer.CanCompare(3.34M).ShouldBeTrue();
            valueComparer.CanCompare(-1.24M).ShouldBeTrue();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_null()
        {
            new DecimalValueComparer()
                .CanCompare(null)
                .ShouldBeFalse();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_not_a_decimal()
        {
            var valueComparer = new DecimalValueComparer();
            valueComparer.CanCompare("x").ShouldBeFalse();
            valueComparer.CanCompare(1).ShouldBeFalse();
            valueComparer.CanCompare(3.14).ShouldBeFalse();
        }

        [Test]
        public void Returns_true_when_the_decimal_values_match()
        {
            var valueComparer = new DecimalValueComparer();
            valueComparer.TheseValuesAreTheSame("3.14", 3.14M).ShouldBeTrue();
            valueComparer.TheseValuesAreTheSame("0", 0M).ShouldBeTrue();
            valueComparer.TheseValuesAreTheSame("-1", -1M).ShouldBeTrue();
        }

        [Test]
        public void Returns_false_when_the_decimal_values_do_not_match()
        {
            var valueComparer = new DecimalValueComparer();
            valueComparer.TheseValuesAreTheSame("-1", 1M).ShouldBeFalse();
            valueComparer.TheseValuesAreTheSame("0", 1M).ShouldBeFalse();
            valueComparer.TheseValuesAreTheSame("100.2874", 100.2873M).ShouldBeFalse();
        }

        [Test]
        public void Returns_false_when_the_expected_value_is_not_a_decimal()
        {
            var valueComparer = new DecimalValueComparer();
            valueComparer.TheseValuesAreTheSame("x", 0M).ShouldBeFalse();
            valueComparer.TheseValuesAreTheSame("", 0M).ShouldBeFalse();
            valueComparer.TheseValuesAreTheSame("-----3", 0M).ShouldBeFalse();
        }
    }
}