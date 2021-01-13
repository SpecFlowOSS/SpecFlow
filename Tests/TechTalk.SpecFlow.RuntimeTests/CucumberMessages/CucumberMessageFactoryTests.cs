using System;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages;
using Xunit;

using static Io.Cucumber.Messages.TestResult.Types;
using static Io.Cucumber.Messages.TestCaseStarted.Types;
using Duration = Io.Cucumber.Messages.Duration;
using Timestamp = Io.Cucumber.Messages.Timestamp;

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

        [Theory(DisplayName = @"BuildTestCaseStarted should return a failure when a non-UTC date has been specified")]
        [InlineData(DateTimeKind.Local)]
        [InlineData(DateTimeKind.Unspecified)]
        public void BuildTestRunResultMessage_NonUtcDate_ShouldReturnFailure(DateTimeKind dateTimeKind)
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, dateTimeKind);
            var pickleId = Guid.NewGuid();
            var platform = new Platform
            {
                Cpu = "x64",
                Implementation = "SpecFlow",
                Os = "Windows",
                Version = "3.1.0"
            };

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseStartedMessage(pickleId, dateTime, platform);

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
            var platform = new Platform
            {
                Cpu = "x64",
                Implementation = "SpecFlow",
                Os = "Windows",
                Version = "3.1.0"
            };

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseStartedMessage(pickleId, dateTime, platform);

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
            var platform = new Platform
            {
                Cpu = "x64",
                Implementation = "SpecFlow",
                Os = "Windows",
                Version = "3.1.0"
            };

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseStartedMessage(pickleId, dateTime, platform);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<TestCaseStarted>>();
        }

        [Fact(DisplayName = @"BuildTestCaseStarted should return a success when the platform information has been specified")]
        public void BuildTestCaseStarted_PlatformInformation_ShouldReturnSuccess()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);
            var pickleId = Guid.NewGuid();
            var platform = new Platform
            {
                Cpu = "x64",
                Implementation = "SpecFlow",
                Os = "Windows",
                Version = "3.1.0"
            };

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseStartedMessage(pickleId, dateTime, platform);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<TestCaseStarted>>()
                  .Which.Result.Platform.Should().BeEquivalentTo(platform);
        }

        [Fact(DisplayName = @"BuildTestCaseStarted should return a failure when the platform information is null")]
        public void BuildTestCaseStarted_PlatformInformationIsNull_ShouldReturnFailue()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);
            var pickleId = Guid.NewGuid();

            // ACT
            var result = cucumberMessageFactory.BuildTestCaseStartedMessage(pickleId, dateTime, null);

            // ASSERT
            result.Should().BeAssignableTo<IFailure>();
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
                Duration = new Duration(){},
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
                Duration = new Duration(),
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
                Duration = new Duration(),
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
                Duration = new Duration() { },
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

        [Fact(DisplayName = @"BuildEnvelopeMessage should return an envelope of type TestCaseFinished")]
        public void BuildEnvelopeMessage_TestCaseFinishedSuccess_ShouldReturnEnvelopeOfTypeTestCaseFinished()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var testCaseFinished = new TestCaseFinished
            {
                PickleId = Guid.NewGuid().ToString(),
                TestResult = new TestResult(),
                Timestamp = new Timestamp()
            };

            // ACT
            var result = cucumberMessageFactory.BuildEnvelopeMessage(new Success<TestCaseFinished>(testCaseFinished));

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<Envelope>>().Which
                  .Result.MessageCase.Should().Be(Envelope.MessageOneofCase.TestCaseFinished);
        }

        [Fact(DisplayName = @"BuildEnvelopeMessage should return an envelope with the passed TestCaseFinished message")]
        public void BuildEnvelopeMessage_TestCaseFinishedSuccess_ShouldReturnEnvelopeWithTestCaseFinishedMessage()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var testCaseFinished = new TestCaseFinished
            {
                PickleId = Guid.NewGuid().ToString(),
                TestResult = new TestResult(),
                Timestamp = new Timestamp()
            };

            // ACT
            var result = cucumberMessageFactory.BuildEnvelopeMessage(new Success<TestCaseFinished>(testCaseFinished));

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<Envelope>>().Which
                  .Result.TestCaseFinished.Should().Be(testCaseFinished);
        }

        [Fact(DisplayName = @"BuildTestRunFinishedMessage should return a TestRunFinished message")]
        public void BuildTestRunFinished_Success_ShouldReturnTestRunFinishedMessage()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);

            // ACT
            var result = cucumberMessageFactory.BuildTestRunFinishedMessage(true, dateTime);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<TestRunFinished>>();
        }

        [Theory(DisplayName = @"BuildTestRunFinishedMessage should return a TestRunFinished message with the specified success value")]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void BuildTestRunFinished_SuccessValue_ShouldReturnTestRunFinishedMessageWithSpecifiedSuccessValue(bool inputSuccess, bool expectedSuccess)
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);

            // ACT
            var result = cucumberMessageFactory.BuildTestRunFinishedMessage(inputSuccess, dateTime);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<TestRunFinished>>().Which
                  .Result.Success.Should().Be(expectedSuccess);
        }

        [Theory]
        [InlineData(DateTimeKind.Local)]
        [InlineData(DateTimeKind.Unspecified)]
        public void BuildTestRunFinished_DateTimeWithInvalidKind_ShouldReturnFailure(DateTimeKind dateTimeKind)
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, dateTimeKind);

            // ACT
            var result = cucumberMessageFactory.BuildTestRunFinishedMessage(true, dateTime);

            // ASSERT
            result.Should().BeAssignableTo<IFailure<TestRunFinished>>();
        }

        [Fact]
        public void BuildTestRunFinished_UtcDateTime_ShouldReturnSuccess()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);

            // ACT
            var result = cucumberMessageFactory.BuildTestRunFinishedMessage(true, dateTime);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<TestRunFinished>>();
        }

        [Fact]
        public void BuildTestRunFinished_UtcDateTime_ShouldReturnSuccessWithSpecifiedTimeStamp()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);

            // ACT
            var result = cucumberMessageFactory.BuildTestRunFinishedMessage(true, dateTime);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess<TestRunFinished>>()
                  .Which.Result.Timestamp.ToDateTime().Should().Be(dateTime);
        }
    }
}
