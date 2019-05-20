using System;
using System.IO;
using FluentAssertions;
using Io.Cucumber.Messages;
using Moq;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages;
using TechTalk.SpecFlow.CucumberMessages.Sinks;
using TechTalk.SpecFlow.FileAccess;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages.Sinks
{
    public class ProtobufFileSinkOutputTests
    {
        [Fact(DisplayName = @"WriteMessage should return success if the ProtobufFileSinkOutput is initialized")]
        public void WriteMessage_Message_ShouldReturnSuccessIfInitialized()
        {
            // ARRANGE
            var message = new TestRunStarted();
            var protobufFileSinkConfiguration = GetProtobufFileSinkConfiguration();
            var binaryFileAccessorMock = GetBinaryFileAccessorMock();
            var protobufFileSinkOutput = new ProtobufFileSinkOutput(binaryFileAccessorMock.Object, protobufFileSinkConfiguration);

            // ACT
            var actualResult = protobufFileSinkOutput.WriteMessage(message);

            // ASSERT
            actualResult.Should().BeOfType<Success>();
        }

        [Fact(DisplayName = @"WriteMessage should write the specified message to OutputStream")]
        public void WriteMessage_Message_ShouldWriteTheSpecifiedMessageToOutputStream()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var message = cucumberMessageFactory.BuildTestRunStartedMessage(DateTime.UtcNow);
            var protobufFileSinkConfiguration = GetProtobufFileSinkConfiguration();
            var writableStream = GetWritableStream();
            var binaryFileAccessorMock = GetBinaryFileAccessorMock(Result.Success<Stream>(writableStream));
            var protobufFileSinkOutput = new ProtobufFileSinkOutput(binaryFileAccessorMock.Object, protobufFileSinkConfiguration);

            // ACT
            protobufFileSinkOutput.WriteMessage(message);

            // ASSERT
            writableStream.ToArray().Length.Should().BeGreaterThan(0);
        }

        public Mock<IBinaryFileAccessor> GetBinaryFileAccessorMock(Result openOrAppendStream = null)
        {
            var binaryFileAccessorMock = new Mock<IBinaryFileAccessor>();
            binaryFileAccessorMock.Setup(m => m.OpenAppendOrCreateFile(It.IsAny<string>()))
                                  .Returns(openOrAppendStream ?? Result.Success<Stream>(GetWritableStream()));
            return binaryFileAccessorMock;
        }

        public Stream GetNotWritableStream()
        {
            return new MemoryStream(new byte[0], false);
        }

        public MemoryStream GetWritableStream()
        {
            return new MemoryStream();
        }

        public ProtobufFileSinkConfiguration GetProtobufFileSinkConfiguration(string targetFilePath = "CucumberMessageQueue")
        {
            return new ProtobufFileSinkConfiguration(targetFilePath);
        }
    }
}
