using System;
using FluentAssertions;
using Google.Protobuf;
using Io.Cucumber.Messages;
using Moq;
using TechTalk.SpecFlow.CucumberMessages.Sinks;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages.Sinks
{
    public class ProtobufFileSinkTests
    {
        [Fact(DisplayName = @"SendMessage should throw an InvalidOperationException if the ProtobufFileSinkOutput could not be initialized")]
        public void SendMessage_Message_ShouldThrowExceptionIfProtobufFileSinkOutputFailedInitialization()
        {
            // ARRANGE
            var protobufFileSinkOutputMock = new Mock<IProtobufFileSinkOutput>();
            protobufFileSinkOutputMock.Setup(m => m.EnsureIsInitialized())
                                      .Returns(false);
            var protobufFileSink = new ProtobufFileSink(protobufFileSinkOutputMock.Object, new object());
            var message = new TestRunStarted();

            // ACT
            Action sendMessage = () => protobufFileSink.SendMessage(message);

            // ASSERT
            sendMessage.Should().Throw<InvalidOperationException>();
        }

        [Fact(DisplayName = @"SendMessage should send a specified message if the ProtobufFileSinkOutput can be initialized successfully")]
        public void SendMessage_Message_ShouldSendMessageIfProtobufFileSinkOutputSucceededInitialization()
        {
            // ARRANGE
            IMessage sentMessage = default;
            var protobufFileSinkOutputMock = new Mock<IProtobufFileSinkOutput>();
            protobufFileSinkOutputMock.Setup(m => m.EnsureIsInitialized())
                                      .Returns(true);
            protobufFileSinkOutputMock.Setup(m => m.WriteMessage(It.IsAny<IMessage>()))
                                      .Callback<IMessage>(msg => sentMessage = msg);
            var protobufFileSink = new ProtobufFileSink(protobufFileSinkOutputMock.Object, new object());
            var message = new TestRunStarted();

            // ACT
            protobufFileSink.SendMessage(message);

            // ASSERT
            sentMessage.Should().NotBeNull();
        }
    }
}
