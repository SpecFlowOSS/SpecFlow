using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultFactoryTests_Pending :  TestResultFactoryTestsBase
    {
        [Fact(DisplayName = @"BuildPendingResult should return a TestResult with status Pending")]
        public void BuildPendingMessage_ValidParameters_ShouldReturnTestResultWithStatusPending()
        {
            // ARRANGE
            var testResultFactory = GetTestResultFactory();
            const TestResult.Types.Status expectedStatus = TestResult.Types.Status.Pending;

            // ACT
            var actualTestResult = testResultFactory.BuildPendingResult(10Lu, "Pending test");

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Status.Should().Be(expectedStatus);
        }

        [Fact(DisplayName = @"BuildPendingResult should return a TestResult with the passed nanoseconds duration")]
        public void BuildPendingResult_Nanoseconds_ShouldReturnTestResultWithNanoseconds()
        {
            // ARRANGE
            var testResultFactory = GetTestResultFactory();
            const ulong expectedNanoseconds = 15Lu;

            // ACT
            var actualTestResult = testResultFactory.BuildPendingResult(expectedNanoseconds, "Pending test");

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.DurationNanoseconds.Should().Be(expectedNanoseconds);
        }

        [Fact(DisplayName = @"BuildPendingResult should return a TestResult with the passed message")]
        public void BuildPendingResult_Message_ShouldReturnTestResultWithMessage()
        {
            // ARRANGE
            var testResultFactory = GetTestResultFactory();
            const string expectedMessage = "This is a test message";

            // ACT
            var actualTestResult = testResultFactory.BuildPendingResult(10Lu, expectedMessage);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Message.Should().Be(expectedMessage);
        }
    }
}
