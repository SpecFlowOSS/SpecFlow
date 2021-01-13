using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultPartFactoryTestsPassed : TestResultPartFactoryTestsBase
    {
        [Fact(DisplayName = @"BuildPassedResult should return a TestResult with status Passed")]
        public void BuildPassedResult_ValidParameters_ShouldReturnTestResultWithStatusPassed()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory();
            const TestResult.Types.Status expectedStatus = TestResult.Types.Status.Passed;

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
            var testResultFactory = GetTestResultPartFactory();
            const ulong expectedNanoseconds = 15Lu;

            // ACT
            var actualTestResult = testResultFactory.BuildPassedResult(expectedNanoseconds);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                .Result.Duration.ToNanoseconds().Should().Be(expectedNanoseconds);
        }

        [Fact(DisplayName = @"BuildPassedResult should return a TestResult with empty message")]
        public void BuildPassedResult_ValidParameters_ShouldReturnTestResultWithEmptyMessage()
        {
            // ARRANGE
            var testResultFactory = GetTestResultPartFactory();
            const string expectedMessage = "";

            // ACT
            var actualTestResult = testResultFactory.BuildPassedResult(10Lu);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ISuccess<TestResult>>().Which
                .Result.Message.Should().Be(expectedMessage);
        }
    }
}
