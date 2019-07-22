using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultFactoryTests_Undefined : TestResultFactoryTestsBase
    {
        [Fact(DisplayName = @"BuildUndefinedResult should return a TestResult with status Undefined")]
        public void BuildUndefinedResult_ValidParameters_ShouldReturnTestResultWithStatusUndefined()
        {
            // ARRANGE
            var testResultFactory = GetTestResultFactory();
            const TestResult.Types.Status expectedStatus = TestResult.Types.Status.Undefined;

            // ACT
            var actualTestResult = testResultFactory.BuildUndefinedResult(10Lu, "Undefined test");

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Status.Should().Be(expectedStatus);
        }

        [Fact(DisplayName = @"BuildUndefinedResult should return a TestResult with the passed nanoseconds duration")]
        public void BuildUndefinedResult_Nanoseconds_ShouldReturnTestResultWithNanoseconds()
        {
            // ARRANGE
            var testResultFactory = GetTestResultFactory();
            const ulong expectedNanoseconds = 15Lu;

            // ACT
            var actualTestResult = testResultFactory.BuildUndefinedResult(expectedNanoseconds, "Undefined test");

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.DurationNanoseconds.Should().Be(expectedNanoseconds);
        }

        [Fact(DisplayName = @"BuildUndefinedResult should return a TestResult with the passed message")]
        public void BuildUndefinedResult_Message_ShouldReturnTestResultWithMessage()
        {
            // ARRANGE
            var testResultFactory = GetTestResultFactory();
            const string expectedMessage = "This is a test message";

            // ACT
            var actualTestResult = testResultFactory.BuildUndefinedResult(10Lu, expectedMessage);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Message.Should().Be(expectedMessage);
        }

    }
}
