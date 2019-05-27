using System;
using FluentAssertions;
using Io.Cucumber.Messages;
using Moq;
using TechTalk.SpecFlow.CucumberMessages;
using TechTalk.SpecFlow.Time;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class CucumberMessageSenderTests
    {
        [Fact(DisplayName = @"SendTestCaseStarted should send a TestCaseStarted message to sink")]
        public void SendTestCaseStarted_ValidParameters_ShouldSendTestRunStartedToSink()
        {
            // ARRANGE
            Wrapper sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<Wrapper>()))
                                   .Callback<Wrapper>(m => sentMessage = m);

            var cucumberMessageSender = new CucumberMessageSender(new UtcDateTimeClock(), new CucumberMessageFactory(), cucumberMessageSinkMock.Object);
            var pickleId = Guid.NewGuid();

            // ACT
            cucumberMessageSender.SendTestCaseStarted(pickleId);

            // ASSERT
            sentMessage.MessageCase.Should().Be(Wrapper.MessageOneofCase.TestRunStarted);
        }

        [Fact(DisplayName = @"SendTestRunStarted should send a TestRunStated message to sink")]
        public void SendTestRunStarted_ShouldSendTestRunStartedToSink()
        {
            // ARRANGE
            Wrapper sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<Wrapper>()))
                                   .Callback<Wrapper>(m => sentMessage = m);

            var cucumberMessageSender = new CucumberMessageSender(new UtcDateTimeClock(), new CucumberMessageFactory(), cucumberMessageSinkMock.Object);

            // ACT
            cucumberMessageSender.SendTestRunStarted();

            // ASSERT
            sentMessage.MessageCase.Should().Be(Wrapper.MessageOneofCase.TestRunStarted);
        }

        [Fact(DisplayName = @"SendTestRunStarted should send a TestRunStated message with correct time stamp to sink")]
        public void SendTestRunStarted_ShouldSendTestRunStartedWithCorrectTimeStampToSink()
        {
            // ARRANGE
            var now = new DateTime(2019, 5, 9, 15, 46, 5, DateTimeKind.Utc);
            var clockMock = new Mock<IClock>();
            clockMock.Setup(m => m.GetNowDateAndTime())
                     .Returns(now);
            clockMock.Setup(m => m.GetToday())
                     .Returns(now.Date);

            Wrapper sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<Wrapper>()))
                                   .Callback<Wrapper>(m => sentMessage = m);

            var cucumberMessageSender = new CucumberMessageSender(clockMock.Object, new CucumberMessageFactory(), cucumberMessageSinkMock.Object);

            // ACT
            cucumberMessageSender.SendTestRunStarted();

            // ASSERT
            sentMessage.MessageCase.Should().Be(Wrapper.MessageOneofCase.TestRunStarted);
            sentMessage.TestRunStarted.Timestamp.ToDateTime().Should().Be(now);
        }

        [Fact(DisplayName = @"SendTestRunStarted should send a TestRunStarted message with SpecFlow as used Cucumber implementation to sink")]
        public void SendTestRunStarted_ShouldSendTestRunStartedWithSpecFlowAsUsedCucumberImplementationToSink()
        {
            // ARRANGE
            const string expectedCucumberImplementation = @"SpecFlow";
            Wrapper sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<Wrapper>()))
                                   .Callback<Wrapper>(m => sentMessage = m);

            var cucumberMessageSender = new CucumberMessageSender(new UtcDateTimeClock(), new CucumberMessageFactory(), cucumberMessageSinkMock.Object);

            // ACT
            cucumberMessageSender.SendTestRunStarted();

            // ASSERT
            sentMessage.MessageCase.Should().Be(Wrapper.MessageOneofCase.TestRunStarted);
            sentMessage.TestRunStarted.CucumberImplementation.Should().Be(expectedCucumberImplementation);
        }
    }
}
