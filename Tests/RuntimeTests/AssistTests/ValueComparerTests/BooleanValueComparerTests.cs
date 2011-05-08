using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    [TestFixture]
    public class BooleanValueComparerTests
    {
        [Test]
        public void Can_compare_if_the_value_is_a_boolean()
        {
            var valueComparer = new BooleanValueComparer();
            valueComparer.CanCompare(true)
                .ShouldBeTrue();
            valueComparer.CanCompare(false)
                .ShouldBeTrue();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_null()
        {
            new BooleanValueComparer()
                .CanCompare(null)
                .ShouldBeFalse();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_not_a_boolean()
        {
            var valueComparer = new BooleanValueComparer();
            valueComparer.CanCompare("x").ShouldBeFalse();
            valueComparer.CanCompare(1).ShouldBeFalse();
            valueComparer.CanCompare(1.34).ShouldBeFalse();
        }

        [Test]
        public void Returns_true_if_the_value_and_string_match()
        {
            var comparer = new BooleanValueComparer();
            comparer.TheseValuesAreTheSame("True", true).ShouldBeTrue();
            comparer.TheseValuesAreTheSame("False", false).ShouldBeTrue();
        }

        [Test]
        public void Returns_false_if_the_value_and_string_do_not_match()
        {
            var comparer = new BooleanValueComparer();
            comparer.TheseValuesAreTheSame("True", false).ShouldBeFalse();
            comparer.TheseValuesAreTheSame("False", true).ShouldBeFalse();
        }

        [Test]
        public void Ignores_casing_of_the_expected_value_when_matching()
        {
            var comparer = new BooleanValueComparer();
            comparer.TheseValuesAreTheSame("true", true).ShouldBeTrue();
            comparer.TheseValuesAreTheSame("FALSE", false).ShouldBeTrue();
            comparer.TheseValuesAreTheSame("truE", true).ShouldBeTrue();
            comparer.TheseValuesAreTheSame("false", false).ShouldBeTrue();
        }
    }
}