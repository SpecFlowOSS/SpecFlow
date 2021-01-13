using System;
using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultPartFactoryTestsAmbiguous : TestResultPartFactoryTestsBase
    {
        [Fact(DisplayName = @"BuildAmbiguousResult should return a TestResult with status Ambiguous")]
        public void BuildAmbiguousResult_ValidParameters_ShouldReturnTestResultWithStatusAmbiguous()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory();
            const TestResult.Types.Status expectedStatus = TestResult.Types.Status.Ambiguous;

            // ACT
            var actualTestResult = testResultFactory.BuildAmbiguousResult(10Lu, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Status.Should().Be(expectedStatus);
        }

        [Fact(DisplayName = @"BuildAmbiguousResult should return a TestResult with the passed nanoseconds duration")]
        public void BuildAmbiguousResult_Nanoseconds_ShouldReturnTestResultWithNanoseconds()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory();
            const ulong expectedNanoseconds = 15Lu;

            // ACT
            var actualTestResult = testResultFactory.BuildAmbiguousResult(expectedNanoseconds, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Duration.ToNanoseconds().Should().Be(expectedNanoseconds);
        }

        [Fact(DisplayName = @"BuildAmbiguousResult should return a TestResult with the passed message")]
        public void BuildAmbiguousResult_Message_ShouldReturnTestResultWithMessage()
        {
            // ARRANGE
            const string expectedMessage = "This is a test message";
            var testResultFactory = GetTestResultPartFactory(expectedMessage: expectedMessage);
            

            // ACT
            var actualTestResult = testResultFactory.BuildAmbiguousResult(10Lu, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Message.Should().Be(expectedMessage);
        }

    }
}
