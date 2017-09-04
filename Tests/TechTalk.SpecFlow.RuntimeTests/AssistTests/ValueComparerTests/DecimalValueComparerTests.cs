using System.Globalization;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist.ValueComparers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueComparerTests
{
    public class DecimalValueComparerTests
    {
        public DecimalValueComparerTests()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        }

        [Fact]
        public void Can_compare_if_the_value_is_a_decimal()
        {
            var valueComparer = new DecimalValueComparer();
            valueComparer.CanCompare(1.0M).Should().BeTrue();
            valueComparer.CanCompare(3.34M).Should().BeTrue();
            valueComparer.CanCompare(-1.24M).Should().BeTrue();
        }

        [Fact]
        public void Cannot_compare_if_the_value_is_null()
        {
            new DecimalValueComparer()
                .CanCompare(null)
                .Should().BeFalse();
        }

        [Fact]
        public void Cannot_compare_if_the_value_is_not_a_decimal()
        {
            var valueComparer = new DecimalValueComparer();
            valueComparer.CanCompare("x").Should().BeFalse();
            valueComparer.CanCompare(1).Should().BeFalse();
            valueComparer.CanCompare(3.14).Should().BeFalse();
        }

        [Fact]
        public void Returns_true_when_the_decimal_values_match()
        {
            var valueComparer = new DecimalValueComparer();
            valueComparer.Compare("3.14", 3.14M).Should().BeTrue();
            valueComparer.Compare("0", 0M).Should().BeTrue();
            valueComparer.Compare("-1", -1M).Should().BeTrue();
        }

        [Fact]
        public void Returns_false_when_the_decimal_values_do_not_match()
        {
            var valueComparer = new DecimalValueComparer();
            valueComparer.Compare("-1", 1M).Should().BeFalse();
            valueComparer.Compare("0", 1M).Should().BeFalse();
            valueComparer.Compare("100.2874", 100.2873M).Should().BeFalse();
        }

        [Fact]
        public void Returns_false_when_the_expected_value_is_not_a_decimal()
        {
            var valueComparer = new DecimalValueComparer();
            valueComparer.Compare("x", 0M).Should().BeFalse();
            valueComparer.Compare("", 0M).Should().BeFalse();
            valueComparer.Compare("-----3", 0M).Should().BeFalse();
        }
    }
}