using System;
using System.IO;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Io.Cucumber.Messages;
using Moq;
using TechTalk.SpecFlow.CommonModels;
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
            var message = new Envelope { TestRunStarted = new TestRunStarted()};
            var protobufFileSinkConfiguration = GetProtobufFileSinkConfiguration();
            var binaryFileAccessorMock = GetBinaryFileAccessorMock();
            var protobufFileNameResolverMock = GetProtobufFileNameResolverMock();

            var protobufFileSinkOutput = new ProtobufFileSinkOutput(binaryFileAccessorMock.Object, protobufFileSinkConfiguration, protobufFileNameResolverMock.Object);

            // ACT
            var actualResult = protobufFileSinkOutput.WriteMessage(message);

            // ASSERT
            actualResult.Should().BeAssignableTo<ISuccess>();
        }

        [Fact(DisplayName = @"WriteMessage should write the specified message to OutputStream")]
        public void WriteMessage_Message_ShouldWriteTheSpecifiedMessageToOutputStream()
        {
            // ARRANGE
            var message = new Envelope
            {
                TestRunStarted = new TestRunStarted
                {
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow)
                }
            };

            var protobufFileSinkConfiguration = GetProtobufFileSinkConfiguration();
            var writableStream = GetWritableStream();
            var binaryFileAccessorMock = GetBinaryFileAccessorMock(Result<Stream>.Success(writableStream));
            var protobufFileNameResolverMock = GetProtobufFileNameResolverMock();

            var protobufFileSinkOutput = new ProtobufFileSinkOutput(binaryFileAccessorMock.Object, protobufFileSinkConfiguration, protobufFileNameResolverMock.Object);

            // ACT
            protobufFileSinkOutput.WriteMessage(message);

            // ASSERT
            writableStream.ToArray().Length.Should().BeGreaterThan(0);
        }

        public Mock<IBinaryFileAccessor> GetBinaryFileAccessorMock(IResult<Stream> openOrAppendStream = null)
        {
            var binaryFileAccessorMock = new Mock<IBinaryFileAccessor>();
            binaryFileAccessorMock.Setup(m => m.OpenAppendOrCreateFile(It.IsAny<string>()))
                                  .Returns(openOrAppendStream ?? Result<Stream>.Success(GetWritableStream()));
            return binaryFileAccessorMock;
        }

        public Mock<IProtobufFileNameResolver> GetProtobufFileNameResolverMock()
        {
            var protobufFileNameResolverMock = new Mock<IProtobufFileNameResolver>();
            protobufFileNameResolverMock.Setup(m => m.Resolve(It.IsAny<string>()))
                                        .Returns<string>(Result<string>.Success);
            return protobufFileNameResolverMock;
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
