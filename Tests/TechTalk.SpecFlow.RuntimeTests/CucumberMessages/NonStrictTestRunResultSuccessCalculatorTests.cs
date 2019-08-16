using FluentAssertions;
using TechTalk.SpecFlow.CucumberMessages;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class NonStrictTestRunResultSuccessCalculatorTests
    {
        [Theory]
        [InlineData(0, 1, 0, 0, 0)]
        [InlineData(0, 0, 0, 1, 0)]
        [InlineData(1, 1, 0, 0, 0)]
        [InlineData(0, 1, 1, 0, 0)]
        [InlineData(0, 1, 1, 1, 0)]
        [InlineData(0, 1, 1, 1, 1)]
        [InlineData(1, 1, 1, 1, 1)]
        public void IsSuccess_NoSuccessData_ShouldReturnFalse(int passed, int failed, int skipped, int ambiguous, int undefined)
        {
            // ARRANGE
            var nonStrictTestRunResultSuccessCalculator = new NonStrictTestRunResultSuccessCalculator();
            var testRunResult = new TestRunResult(
                passed + failed + skipped + ambiguous + undefined,
                passed,
                failed,
                skipped,
                ambiguous,
                undefined);

            // ACT
            bool isSuccess = nonStrictTestRunResultSuccessCalculator.IsSuccess(testRunResult);

            // ASSERT
            isSuccess.Should().BeFalse();
        }

        [Theory]
        [InlineData(0, 0, 1, 0, 0)]
        [InlineData(0, 0, 0, 0, 1)]
        [InlineData(1, 0, 0, 0, 1)]
        [InlineData(1, 0, 0, 0, 0)]
        public void IsSuccess_SuccessData_ShouldReturnTrue(int passed, int failed, int skipped, int ambiguous, int undefined)
        {
            // ARRANGE
            var nonStrictTestRunResultSuccessCalculator = new NonStrictTestRunResultSuccessCalculator();
            var testRunResult = new TestRunResult(
                passed + failed + skipped + ambiguous + undefined,
                passed,
                failed,
                skipped,
                ambiguous,
                undefined);

            // ACT
            bool isSuccess = nonStrictTestRunResultSuccessCalculator.IsSuccess(testRunResult);

            // ASSERT
            isSuccess.Should().BeTrue();
        }
    }
}
