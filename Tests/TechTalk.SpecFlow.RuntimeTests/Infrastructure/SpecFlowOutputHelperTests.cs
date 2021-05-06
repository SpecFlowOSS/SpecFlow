using Moq;
using TechTalk.SpecFlow.Events;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    public class SpecFlowOutputHelperTests
    {
        private Mock<ITestThreadExecutionEventPublisher> _testThreadExecutionEventPublisher;

        [Fact]
        public void Should_publish_output_added_event()
        {
            var outputHelper = CreateSpecFlowOutputHelper();
            var message = "This is a sample output message!";

            outputHelper.WriteLine(message);

            _testThreadExecutionEventPublisher.Verify(ep => ep.PublishEvent(It.Is<OutputAddedEvent>(m => m.Text == message)), Times.Once);
        }

        [Fact]
        public void Should_publish_attachment_added_event()
        {
            var outputHelper = CreateSpecFlowOutputHelper();
            var filePath = @"C:\Temp\sample.png";

            outputHelper.AddAttachment(filePath);

            _testThreadExecutionEventPublisher.Verify(ep => ep.PublishEvent(It.Is<AttachmentAddedEvent>(m => m.FilePath == filePath)), Times.Once);
        }

        private SpecFlowOutputHelper CreateSpecFlowOutputHelper()
        {
            _testThreadExecutionEventPublisher = new Mock<ITestThreadExecutionEventPublisher>();
            var traceListenerMock = new Mock<ITraceListener>();
            var attachmentHandlerMock = new Mock<ISpecFlowAttachmentHandler>();

            return new SpecFlowOutputHelper(_testThreadExecutionEventPublisher.Object, traceListenerMock.Object, attachmentHandlerMock.Object);
        }
    }
}
