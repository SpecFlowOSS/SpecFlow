using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    
    public class BoolValueComparerTests
    {
        [Fact]
        public void Can_compare_if_the_value_is_a_bool()
        {
            var valueComparer = new BoolValueComparer();
            valueComparer.CanCompare(true)
                .Should().BeTrue();
            valueComparer.CanCompare(false)
                .Should().BeTrue();
        }

        [Fact]
        public void Cannot_compare_if_the_value_is_null()
        {
            new BoolValueComparer()
                .CanCompare(null)
                .Should().BeFalse();
        }

        [Fact]
        public void Cannot_compare_if_the_value_is_not_a_bool()
        {
            var valueComparer = new BoolValueComparer();
            valueComparer.CanCompare("x").Should().BeFalse();
            valueComparer.CanCompare(1).Should().BeFalse();
            valueComparer.CanCompare(1.34).Should().BeFalse();
        }

        [Fact]
        public void Returns_true_if_the_value_and_string_match()
        {
            var comparer = new BoolValueComparer();
            comparer.Compare("True", true).Should().BeTrue();
            comparer.Compare("False", false).Should().BeTrue();
        }

        [Fact]
        public void Returns_false_if_the_value_and_string_do_not_match()
        {
            var comparer = new BoolValueComparer();
            comparer.Compare("True", false).Should().BeFalse();
            comparer.Compare("False", true).Should().BeFalse();
        }

        [Fact]
        public void Ignores_casing_of_the_expected_value_when_matching()
        {
            var comparer = new BoolValueComparer();
            comparer.Compare("true", true).Should().BeTrue();
            comparer.Compare("FALSE", false).Should().BeTrue();
            comparer.Compare("truE", true).Should().BeTrue();
            comparer.Compare("false", false).Should().BeTrue();
        }
    }
}