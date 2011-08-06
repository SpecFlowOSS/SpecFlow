using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    [TestFixture, SetCulture("en-US")]
    public class SingleValueComparerTests
    {
        [Test]
        public void Can_compare_if_the_value_is_a_single()
        {
            var valueComparer = new SingleValueComparer();
            valueComparer.CanCompare(1.0F).ShouldBeTrue();
            valueComparer.CanCompare(3.34F).ShouldBeTrue();
            valueComparer.CanCompare(-1.24F).ShouldBeTrue();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_null()
        {
            new SingleValueComparer()
                .CanCompare(null)
                .ShouldBeFalse();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_not_a_single()
        {
            var valueComparer = new SingleValueComparer();
            valueComparer.CanCompare("x").ShouldBeFalse();
            valueComparer.CanCompare(1).ShouldBeFalse();
            valueComparer.CanCompare(3.14M).ShouldBeFalse();
        }

        [Test]
        public void Returns_true_when_the_single_values_match()
        {
            var valueComparer = new SingleValueComparer();
            valueComparer.TheseValuesAreTheSame("3.14", 3.14F).ShouldBeTrue();
            valueComparer.TheseValuesAreTheSame("0", 0.0F).ShouldBeTrue();
            valueComparer.TheseValuesAreTheSame("-1", -1.0F).ShouldBeTrue();
        }

        [Test]
        public void Returns_false_when_the_single_values_do_not_match()
        {
            var valueComparer = new SingleValueComparer();
            valueComparer.TheseValuesAreTheSame("-1", 1.0F).ShouldBeFalse();
            valueComparer.TheseValuesAreTheSame("0", 1.0F).ShouldBeFalse();
            valueComparer.TheseValuesAreTheSame("100.2874", 100.2873F).ShouldBeFalse();
        }

        [Test]
        public void Returns_false_when_the_expected_value_is_not_a_single()
        {
            var valueComparer = new SingleValueComparer();
            valueComparer.TheseValuesAreTheSame("x", 0.0F).ShouldBeFalse();
            valueComparer.TheseValuesAreTheSame("", 0.0F).ShouldBeFalse();
            valueComparer.TheseValuesAreTheSame("-----3", 0F).ShouldBeFalse();
        }
    }
}