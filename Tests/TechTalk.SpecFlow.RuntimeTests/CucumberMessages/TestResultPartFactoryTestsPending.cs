using System;
using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultPartFactoryTestsPending :  TestResultPartFactoryTestsBase
    {
        [Fact(DisplayName = @"BuildPendingResult should return a TestResult with status Pending")]
        public void BuildPendingMessage_ValidParameters_ShouldReturnTestResultWithStatusPending()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory(expectedMessage: "Pending test");
            const TestResult.Types.Status expectedStatus = TestResult.Types.Status.Pending;

            // ACT
            var actualTestResult = testResultFactory.BuildPendingResult(10Lu, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Status.Should().Be(expectedStatus);
        }

        [Fact(DisplayName = @"BuildPendingResult should return a TestResult with the passed nanoseconds duration")]
        public void BuildPendingResult_Nanoseconds_ShouldReturnTestResultWithNanoseconds()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory(expectedMessage: "Pending test");
            const ulong expectedNanoseconds = 15Lu;

            // ACT
            var actualTestResult = testResultFactory.BuildPendingResult(expectedNanoseconds, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Duration.ToNanoseconds().Should().Be(expectedNanoseconds);
        }

        [Fact(DisplayName = @"BuildPendingResult should return a TestResult with the passed message")]
        public void BuildPendingResult_Message_ShouldReturnTestResultWithMessage()
        {
            // ARRANGE
            const string expectedMessage = "This is a test message";
            var testResultFactory = GetTestResultPartFactory(expectedMessage: expectedMessage);
            

            // ACT
            var actualTestResult = testResultFactory.BuildPendingResult(10Lu, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Message.Should().Be(expectedMessage);
        }
    }
}
