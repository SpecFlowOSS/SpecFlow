using FluentAssertions;
using TechTalk.SpecFlow.CucumberMessages;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class StrictTestRunResultSuccessCalculatorTests
    {
        [Theory]
        [InlineData(0, 1, 0, 0, 0)]
        [InlineData(0, 0, 1, 0, 0)]
        [InlineData(0, 0, 0, 1, 0)]
        [InlineData(0, 0, 0, 0, 1)]
        [InlineData(1, 0, 0, 0, 1)]
        [InlineData(0, 1, 1, 0, 0)]
        [InlineData(0, 1, 1, 1, 0)]
        [InlineData(0, 1, 1, 1, 1)]
        [InlineData(1, 1, 1, 1, 1)]
        public void IsSuccess_NoSuccessData_ShouldReturnFalse(int passed, int failed, int skipped, int ambiguous, int undefined)
        {
            // ARRANGE
            var strictTestRunResultSuccessCalculator = new StrictTestRunResultSuccessCalculator();
            var testRunResult = new TestRunResult(
                passed + failed + skipped + ambiguous + undefined,
                passed,
                failed,
                skipped,
                ambiguous,
                undefined);

            // ACT
            bool isSuccess = strictTestRunResultSuccessCalculator.IsSuccess(testRunResult);

            // ASSERT
            isSuccess.Should().BeFalse();
        }
    }
}
