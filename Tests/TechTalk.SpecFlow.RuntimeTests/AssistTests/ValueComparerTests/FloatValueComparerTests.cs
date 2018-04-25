using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    public class FloatValueComparerTests
    {
        public FloatValueComparerTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        [Fact]
        public void Can_compare_if_the_value_is_a_single()
        {
            var valueComparer = new FloatValueComparer();
            valueComparer.CanCompare(1.0F).Should().BeTrue();
            valueComparer.CanCompare(3.34F).Should().BeTrue();
            valueComparer.CanCompare(-1.24F).Should().BeTrue();
        }

        [Fact]
        public void Cannot_compare_if_the_value_is_null()
        {
            new FloatValueComparer()
                .CanCompare(null)
                .Should().BeFalse();
        }

        [Fact]
        public void Cannot_compare_if_the_value_is_not_a_single()
        {
            var valueComparer = new FloatValueComparer();
            valueComparer.CanCompare("x").Should().BeFalse();
            valueComparer.CanCompare(1).Should().BeFalse();
            valueComparer.CanCompare(3.14M).Should().BeFalse();
        }

        [Fact]
        public void Returns_true_when_the_single_values_match()
        {
            var valueComparer = new FloatValueComparer();
            valueComparer.Compare("3.14", 3.14F).Should().BeTrue();
            valueComparer.Compare("0", 0.0F).Should().BeTrue();
            valueComparer.Compare("-1", -1.0F).Should().BeTrue();
        }

        [Fact]
        public void Returns_false_when_the_single_values_do_not_match()
        {
            var valueComparer = new FloatValueComparer();
            valueComparer.Compare("-1", 1.0F).Should().BeFalse();
            valueComparer.Compare("0", 1.0F).Should().BeFalse();
            valueComparer.Compare("100.2874", 100.2873F).Should().BeFalse();
        }

        [Fact]
        public void Returns_false_when_the_expected_value_is_not_a_single()
        {
            var valueComparer = new FloatValueComparer();
            valueComparer.Compare("x", 0.0F).Should().BeFalse();
            valueComparer.Compare("", 0.0F).Should().BeFalse();
            valueComparer.Compare("-----3", 0F).Should().BeFalse();
        }
    }
}