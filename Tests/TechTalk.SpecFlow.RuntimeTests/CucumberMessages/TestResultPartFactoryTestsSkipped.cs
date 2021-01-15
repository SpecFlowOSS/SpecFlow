using System;
using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultPartFactoryTestsSkipped : TestResultPartFactoryTestsBase
    {
        [Fact(DisplayName = @"BuildSkippedResult should return a TestResult with status skipped")]
        public void BuildSkippedResult_ValidParameters_ShouldReturnTestResultWithStatusSkipped()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory();
            const TestResult.Types.Status expectedStatus = TestResult.Types.Status.Skipped;

            // ACT
            var actualTestResult = testResultFactory.BuildSkippedResult(10Lu);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                .Result.Status.Should().Be(expectedStatus);
        }

        [Fact(DisplayName = @"BuildSkippedResult should return a TestResult with the Skipped nanoseconds duration")]
        public void BuildSkippedResult_Nanoseconds_ShouldReturnTestResultWithCorrectNanoseconds()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory();
            const ulong expectedNanoseconds = 15Lu;

            // ACT
            var actualTestResult = testResultFactory.BuildSkippedResult(expectedNanoseconds);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                .Result.Duration.ToNanoseconds().Should().Be(expectedNanoseconds);
        }

        [Fact(DisplayName = @"BuildSkippedResult should return a TestResult with empty message")]
        public void BuildSkippedResult_ValidParameters_ShouldReturnTestResultWithEmptyMessage()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory();
            
            // ACT
            var actualTestResult = testResultFactory.BuildSkippedResult(10Lu);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                .Result.Message.Should().BeEmpty();
        }
    }
}
