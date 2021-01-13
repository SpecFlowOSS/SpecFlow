using System;
using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultPartFactoryTestsUndefined : TestResultPartFactoryTestsBase
    {
        [Fact(DisplayName = @"BuildUndefinedResult should return a TestResult with status Undefined")]
        public void BuildUndefinedResult_ValidParameters_ShouldReturnTestResultWithStatusUndefined()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory(expectedMessage: "Undefined test");
            const TestResult.Types.Status expectedStatus = TestResult.Types.Status.Undefined;

            // ACT
            var actualTestResult = testResultFactory.BuildUndefinedResult(10Lu, null, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Status.Should().Be(expectedStatus);
        }

        [Fact(DisplayName = @"BuildUndefinedResult should return a TestResult with the passed nanoseconds duration")]
        public void BuildUndefinedResult_Nanoseconds_ShouldReturnTestResultWithNanoseconds()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory(expectedMessage: "Undefined test");
            const ulong expectedNanoseconds = 15Lu;

            // ACT
            var actualTestResult = testResultFactory.BuildUndefinedResult(expectedNanoseconds, null, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Duration.ToNanoseconds().Should().Be(expectedNanoseconds);
        }

        [Fact(DisplayName = @"BuildUndefinedResult should return a TestResult with the passed message")]
        public void BuildUndefinedResult_Message_ShouldReturnTestResultWithMessage()
        {
            // ARRANGE
            const string expectedMessage = "This is a test message";
            var testResultFactory = GetTestResultPartFactory(expectedMessage: expectedMessage);
            

            // ACT
            var actualTestResult = testResultFactory.BuildUndefinedResult(10Lu, null, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Message.Should().Be(expectedMessage);
        }

    }
}
