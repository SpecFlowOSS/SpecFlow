using System;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class CucumberMessageFactoryTests
    {
        [Fact(DisplayName = @"BuildTestRunResultMessage should return a TestRunResult message object")]
        public void BuildTestRunResultMessage_DateTime_ShouldReturnTestRunResultMessageObject()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);

            // ACT
            var actualTestRunStartedMessageResult = cucumberMessageFactory.BuildTestRunStartedMessage(dateTime);

            // ASSERT
            actualTestRunStartedMessageResult.Should().BeAssignableTo<ISuccess<TestRunStarted>>();
        }

        [Fact(DisplayName = @"BuildTestRunResultMessage should return a TestRunResult message object with the specified date and time")]
        public void BuildTestRunResultMessage_DateTime_ShouldReturnTestRunResultMessageObjectWithSpecifiedDateAndTime()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);

            // ACT
            var actualTestRunStartedMessageResult = cucumberMessageFactory.BuildTestRunStartedMessage(dateTime);

            // ASSERT
            actualTestRunStartedMessageResult.Should().BeAssignableTo<ISuccess<TestRunStarted>>()
                                             .Which.Result.Timestamp.ToDateTime().Should().Be(dateTime);
        }

        [Fact(DisplayName = @"BuildTestRunResultMessage should return a TestRunResult message object with SpecFlow as used Cucumber implementation")]
        public void BuildTestRunResultMessage_ValidParameters_ShouldReturnTestRunResultMessageObjectWithSpecFlowAsUsedCucumberImplementation()
        {
            // ARRANGE
            const string expectedCucumberImplementation = @"SpecFlow";
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);

            // ACT
            var actualTestRunStartedMessageResult = cucumberMessageFactory.BuildTestRunStartedMessage(dateTime);

            // ASSERT

            actualTestRunStartedMessageResult.Should().BeAssignableTo<ISuccess<TestRunStarted>>()
                                             .Which.Result.CucumberImplementation.Should().Be(expectedCucumberImplementation);
        }

        [Theory(DisplayName = @"BuildTestCaseStarted should return a failure when a non-UTC date has been specified")]
        [InlineData(DateTimeKind.Local)]
        [InlineData(DateTimeKind.Unspecified)]
        public void BuildTestRunResultMessage_NonUtcDate_ShouldReturnFailure(DateTimeKind dateTimeKind)
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, dateTimeKind);
            var pickleId = Guid.NewGuid();

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseStartedMessage(pickleId, dateTime);

            // ASSERT
            result.Should().BeAssignableTo<IFailure>();
        }

        [Fact(DisplayName = @"BuildTestCaseStarted should return a message with the correct pickle ID")]
        public void BuildTestCaseStarted_ValidData_ShouldReturnMessageWithCorrectPickleId()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);
            var pickleId = Guid.NewGuid();

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseStartedMessage(pickleId, dateTime);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<TestCaseStarted>>().Which
                  .Result.PickleId.Should().Be(pickleId.ToString("D"));
        }

        [Fact(DisplayName = @"BuildTestCaseStarted should return a success when a UTC date has been specified")]
        public void BuildTestCaseStarted_UtcDate_ShouldReturnSuccess()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);
            var pickleId = Guid.NewGuid();

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseStartedMessage(pickleId, dateTime);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<TestCaseStarted>>();
        }

        [Fact(DisplayName = @"BuildTestCaseFinished should return a message with the correct pickle ID")]
        public void BuildTestCaseFinished_PickleId_ShouldReturnMessageWithCorrectPickleId()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);
            var pickleId = Guid.NewGuid();
            var testResult = new TestResult
            {
                DurationNanoseconds = 1000,
                Message = "",
                Status = Status.Passed
            };

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseFinishedMessage(pickleId, dateTime, testResult);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<TestCaseFinished>>().Which
                  .Result.PickleId.Should().Be(pickleId.ToString("D"));
        }

        [Fact(DisplayName = @"BuildTestCaseFinished should return a message with the correct time stamp when a UTC time stamp is passed")]
        public void BuildTestCaseFinished_UtcDateTime_ShouldReturnMessageWithCorrectTimeStamp()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);
            var pickleId = Guid.NewGuid();
            var testResult = new TestResult
            {
                DurationNanoseconds = 1000,
                Message = "",
                Status = Status.Passed
            };

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseFinishedMessage(pickleId, dateTime, testResult);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<TestCaseFinished>>().Which
                  .Result.Timestamp.ToDateTime().Should().Be(dateTime);
        }

        [Theory(DisplayName = @"BuildTestCaseFinished should return a failure when a non-UTC time stamp is passed")]
        [InlineData(DateTimeKind.Local)]
        [InlineData(DateTimeKind.Unspecified)]
        public void BuildTestCaseFinished_NonUtcDateTime_ShouldReturnFailure(DateTimeKind dateTimeKind)
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, dateTimeKind);
            var pickleId = Guid.NewGuid();
            var testResult = new TestResult
            {
                DurationNanoseconds = 1000,
                Message = "",
                Status = Status.Passed
            };

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseFinishedMessage(pickleId, dateTime, testResult);

            // ASSERT
            result.Should().BeAssignableTo<IFailure<TestCaseFinished>>();
        }

        [Fact(DisplayName = @"BuildTestCaseFinished should return a message with the correct TestResult")]
        public void BuildTestCaseFinished_TestResult_ShouldReturnMessageWithCorrectTestResult()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);
            var pickleId = Guid.NewGuid();
            var testResult = new TestResult
            {
                DurationNanoseconds = 1000,
                Message = "",
                Status = Status.Passed
            };

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseFinishedMessage(pickleId, dateTime, testResult);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<TestCaseFinished>>().Which
                  .Result.TestResult.Should().Be(testResult);
        }

        [Fact(DisplayName = @"BuildTestCaseFinished should return a failure with exception information when null has been specified as TestResult")]
        public void BuildTestCaseFinished_NullTestResult_ShouldReturnExceptionFailure()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);
            var pickleId = Guid.NewGuid();

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseFinishedMessage(pickleId, dateTime, default);

            // ASSERT
            result.Should().BeAssignableTo<ExceptionFailure<TestCaseFinished>>().Which
                  .Exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = @"BuildWrapperMessage should return a wrapper of type TestCaseFinished")]
        public void BuildWrapperMessage_TestCaseFinishedSuccess_ShouldReturnWrapperOfTypeTestCaseFinished()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);
            var testCaseFinished = new TestCaseFinished
            {
                PickleId = Guid.NewGuid().ToString(),
                TestResult = new TestResult(),
                Timestamp = Timestamp.FromDateTime(dateTime)
            };

            // ACT
            var result = cucumberMessageFactory.BuildWrapperMessage(new Success<TestCaseFinished>(testCaseFinished));

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<Wrapper>>().Which
                  .Result.MessageCase.Should().Be(Wrapper.MessageOneofCase.TestCaseFinished);
        }

        [Fact(DisplayName = @"BuildWrapperMessage should return a wrapper with the passed TestCaseFinished message")]
        public void BuildWrapperMessage_TestCaseFinishedSuccess_ShouldReturnWrapperWithTestCaseFinishedMessage()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);
            var testCaseFinished = new TestCaseFinished
            {
                PickleId = Guid.NewGuid().ToString(),
                TestResult = new TestResult(),
                Timestamp = Timestamp.FromDateTime(dateTime)
            };

            // ACT
            var result = cucumberMessageFactory.BuildWrapperMessage(new Success<TestCaseFinished>(testCaseFinished));

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<Wrapper>>().Which
                  .Result.TestCaseFinished.Should().Be(testCaseFinished);
        }
    }
}
