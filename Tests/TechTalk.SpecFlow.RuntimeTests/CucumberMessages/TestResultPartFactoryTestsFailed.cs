using System;
using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultPartFactoryTestsFailed : TestResultPartFactoryTestsBase
    {
        [Fact(DisplayName = @"BuildFailedResult should return a TestResult with status Failed")]
        public void BuildFailedResult_ValidParameters_ShouldReturnTestResultWithStatusFailed()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory(expectedMessage: "Test Message");
            const TestResult.Types.Status expectedStatus = TestResult.Types.Status.Failed;

            // ACT
            var actualTestResult = testResultFactory.BuildFailedResult(10Lu, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Status.Should().Be(expectedStatus);
        }

        [Fact(DisplayName = @"BuildFailedResult should return a TestResult with the passed nanoseconds duration")]
        public void BuildFailedResult_Nanoseconds_ShouldReturnTestResultWithNanoseconds()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory(expectedMessage: "Test Message");
            const ulong expectedNanoseconds = 15Lu;

            // ACT
            var actualTestResult = testResultFactory.BuildFailedResult(expectedNanoseconds, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Duration.ToNanoseconds().Should().Be(expectedNanoseconds);
        }

        [Fact(DisplayName = @"BuildFailedResult should return a TestResult with the passed message")]
        public void BuildFailedResult_Message_ShouldReturnTestResultWithMessage()
        {
            // ARRANGE
            const string expectedMessage = "This is a test message";
            var testResultFactory = GetTestResultPartFactory(expectedMessage: expectedMessage);
            

            // ACT
            var actualTestResult = testResultFactory.BuildFailedResult(10Lu, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Message.Should().Be(expectedMessage);
        }
    }
}
