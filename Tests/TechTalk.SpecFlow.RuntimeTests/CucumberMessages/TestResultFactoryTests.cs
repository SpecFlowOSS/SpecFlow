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
            var testResultFactory = new TestResultFactory();
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
            var testResultFactory = new TestResultFactory();
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
            var testResultFactory = new TestResultFactory();
            const string expectedMessage = "";

            // ACT
            var actualTestResult = testResultFactory.BuildPassedResult(10Lu);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Message.Should().Be(expectedMessage);
        }
    }
}
