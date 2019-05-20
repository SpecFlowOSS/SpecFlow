using System;
using FluentAssertions;
using Google.Protobuf;
using Io.Cucumber.Messages;
using Moq;
using TechTalk.SpecFlow.CucumberMessages;
using TechTalk.SpecFlow.Time;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class CucumberMessageSenderTests
    {
        [Fact(DisplayName = @"SendTestRunStarted should send a TestRunStated message to sink")]
        public void SendTestRunStarted_ShouldSendTestRunStartedToSink()
        {
            // ARRANGE
            IMessage sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<IMessage>()))
                                   .Callback<IMessage>(m => sentMessage = m);

            var cucumberMessageSender = new CucumberMessageSender(new UtcDateTimeClock(), new CucumberMessageFactory(), cucumberMessageSinkMock.Object);

            // ACT
            cucumberMessageSender.SendTestRunStarted();

            // ASSERT
            sentMessage.Should().BeOfType<TestRunStarted>();
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

            IMessage sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<IMessage>()))
                                   .Callback<IMessage>(m => sentMessage = m);

            var cucumberMessageSender = new CucumberMessageSender(clockMock.Object, new CucumberMessageFactory(), cucumberMessageSinkMock.Object);

            // ACT
            cucumberMessageSender.SendTestRunStarted();

            // ASSERT
            sentMessage.Should().BeOfType<TestRunStarted>()
                       .Which.Timestamp.ToDateTime().Should().Be(now);
        }

        [Fact(DisplayName = @"SendTestRunStarted should send a TestRunStarted message with SpecFlow as used Cucumber implementation to sink")]
        public void SendTestRunStarted_ShouldSendTestRunStartedWithSpecFlowAsUsedCucumberImplementationToSink()
        {
            // ARRANGE
            const string expectedCucumberImplementation = @"SpecFlow";
            IMessage sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<IMessage>()))
                                   .Callback<IMessage>(m => sentMessage = m);

            var cucumberMessageSender = new CucumberMessageSender(new UtcDateTimeClock(), new CucumberMessageFactory(), cucumberMessageSinkMock.Object);

            // ACT
            cucumberMessageSender.SendTestRunStarted();

            // ASSERT
            sentMessage.Should().BeOfType<TestRunStarted>()
                       .Which.CucumberImplementation.Should().Be(expectedCucumberImplementation);
        }
    }
}
