using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    [TestFixture]
    public class BoolValueComparerTests
    {
        [Test]
        public void Can_compare_if_the_value_is_a_bool()
        {
            var valueComparer = new BoolValueComparer();
            valueComparer.CanCompare(true)
                .Should().BeTrue();
            valueComparer.CanCompare(false)
                .Should().BeTrue();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_null()
        {
            new BoolValueComparer()
                .CanCompare(null)
                .Should().BeFalse();
        }

        [Test]
        public void Cannot_compare_if_the_value_is_not_a_bool()
        {
            var valueComparer = new BoolValueComparer();
            valueComparer.CanCompare("x").Should().BeFalse();
            valueComparer.CanCompare(1).Should().BeFalse();
            valueComparer.CanCompare(1.34).Should().BeFalse();
        }

        [Test]
        public void Returns_true_if_the_value_and_string_match()
        {
            var comparer = new BoolValueComparer();
            comparer.TheseValuesAreTheSame("True", true).Should().BeTrue();
            comparer.TheseValuesAreTheSame("False", false).Should().BeTrue();
        }

        [Test]
        public void Returns_false_if_the_value_and_string_do_not_match()
        {
            var comparer = new BoolValueComparer();
            comparer.TheseValuesAreTheSame("True", false).Should().BeFalse();
            comparer.TheseValuesAreTheSame("False", true).Should().BeFalse();
        }

        [Test]
        public void Ignores_casing_of_the_expected_value_when_matching()
        {
            var comparer = new BoolValueComparer();
            comparer.TheseValuesAreTheSame("true", true).Should().BeTrue();
            comparer.TheseValuesAreTheSame("FALSE", false).Should().BeTrue();
            comparer.TheseValuesAreTheSame("truE", true).Should().BeTrue();
            comparer.TheseValuesAreTheSame("false", false).Should().BeTrue();
        }
    }
}