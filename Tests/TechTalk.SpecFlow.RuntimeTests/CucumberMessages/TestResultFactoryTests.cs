using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultFactoryTests
    {
        [Fact(DisplayName = @"BuildPassedResult should return a TestResult with status Passed")]
        public void BuildPassedResult_ValidParameters_ShouldReturnTestResultWithStatusPassed()
        {
            // ARRANGE
            var testResultFactory = new TestResultFactory(new TestErrorMessageFactory());
            const Status expectedStatus = Status.Passed;

            // ACT
            var actualTestResult = testResultFactory.BuildPassedResult(10Lu);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Status.Should().Be(expectedStatus);
        }

        [Fact(DisplayName = @"BuildPassedResult should return a TestResult with the passed nanoseconds duration")]
        public void BuildPassedResult_Nanoseconds_ShouldReturnTestResultWithCorrectNanoseconds()
        {
            // ARRANGE
            var testResultFactory = new TestResultFactory(new TestErrorMessageFactory());
            const ulong expectedNanoseconds = 15Lu;

            // ACT
            var actualTestResult = testResultFactory.BuildPassedResult(expectedNanoseconds);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.DurationNanoseconds.Should().Be(expectedNanoseconds);
        }

        [Fact(DisplayName = @"BuildPassedResult should return a TestResult with empty message")]
        public void BuildPassedResult_ValidParameters_ShouldReturnTestResultWithEmptyMessage()
        {
            // ARRANGE
            var testResultFactory = new TestResultFactory(new TestErrorMessageFactory());
            const string expectedMessage = "";

            // ACT
            var actualTestResult = testResultFactory.BuildPassedResult(10Lu);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Message.Should().Be(expectedMessage);
        }

        [Fact(DisplayName = @"BuildFailedResult should return a TestResult with status Failed")]
        public void BuildFailedResult_ValidParameters_ShouldReturnTestResultWithStatusFailed()
        {
            // ARRANGE
            var testResultFactory = new TestResultFactory(new TestErrorMessageFactory());
            const Status expectedStatus = Status.Failed;

            // ACT
            var actualTestResult = testResultFactory.BuildFailedResult(10Lu, "Test Message");

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Status.Should().Be(expectedStatus);
        }

        [Fact(DisplayName = @"BuildFailedResult should return a TestResult with the passed nanoseconds duration")]
        public void BuildFailedResult_Nanoseconds_ShouldReturnTestResultWithNanoseconds()
        {
            // ARRANGE
            var testResultFactory = new TestResultFactory(new TestErrorMessageFactory());
            const ulong expectedNanoseconds = 15Lu;

            // ACT
            var actualTestResult = testResultFactory.BuildFailedResult(expectedNanoseconds, "Test Message");

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.DurationNanoseconds.Should().Be(expectedNanoseconds);
        }

        [Fact(DisplayName = @"BuildFailedResult should return a TestResult with the passed message")]
        public void BuildFailedResult_Message_ShouldReturnTestResultWithMessage()
        {
            // ARRANGE
            var testResultFactory = new TestResultFactory(new TestErrorMessageFactory());
            const string expectedMessage = "This is a test message";

            // ACT
            var actualTestResult = testResultFactory.BuildFailedResult(10Lu, expectedMessage);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Message.Should().Be(expectedMessage);
        }

        [Fact(DisplayName = @"BuildPendingResult should return a TestResult with status Pending")]
        public void BuildPendingMessage_ValidParameters_ShouldReturnTestResultWithStatusPending()
        {
            // ARRANGE
            var testResultFactory = new TestResultFactory(new TestErrorMessageFactory());
            const Status expectedStatus = Status.Pending;

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
            var testResultFactory = new TestResultFactory(new TestErrorMessageFactory());
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
            var testResultFactory = new TestResultFactory(new TestErrorMessageFactory());
            const string expectedMessage = "This is a test message";

            // ACT
            var actualTestResult = testResultFactory.BuildPendingResult(10Lu, expectedMessage);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Message.Should().Be(expectedMessage);
        }
    }
}
