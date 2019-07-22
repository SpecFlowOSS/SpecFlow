using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultFactoryTests_Ambiguous : TestResultFactoryTestsBase
    {
        [Fact(DisplayName = @"BuildAmbiguousResult should return a TestResult with status Ambiguous")]
        public void BuildAmbiguousResult_ValidParameters_ShouldReturnTestResultWithStatusAmbiguous()
        {
            // ARRANGE
            var testResultFactory = GetTestResultFactory();
            const TestResult.Types.Status expectedStatus = TestResult.Types.Status.Ambiguous;

            // ACT
            var actualTestResult = testResultFactory.BuildAmbiguousResult(10Lu, "Ambiguous test");

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Status.Should().Be(expectedStatus);
        }

        [Fact(DisplayName = @"BuildAmbiguousResult should return a TestResult with the passed nanoseconds duration")]
        public void BuildAmbiguousResult_Nanoseconds_ShouldReturnTestResultWithNanoseconds()
        {
            // ARRANGE
            var testResultFactory = GetTestResultFactory();
            const ulong expectedNanoseconds = 15Lu;

            // ACT
            var actualTestResult = testResultFactory.BuildAmbiguousResult(expectedNanoseconds, "Ambiguous test");

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.DurationNanoseconds.Should().Be(expectedNanoseconds);
        }

        [Fact(DisplayName = @"BuildAmbiguousResult should return a TestResult with the passed message")]
        public void BuildAmbiguousResult_Message_ShouldReturnTestResultWithMessage()
        {
            // ARRANGE
            var testResultFactory = GetTestResultFactory();
            const string expectedMessage = "This is a test message";

            // ACT
            var actualTestResult = testResultFactory.BuildAmbiguousResult(10Lu, expectedMessage);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                            .Result.Message.Should().Be(expectedMessage);
        }

    }
}
