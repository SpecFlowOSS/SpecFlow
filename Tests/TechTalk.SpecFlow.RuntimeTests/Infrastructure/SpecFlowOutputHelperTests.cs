using System.Linq;
using BoDi;
using Moq;
using TechTalk.SpecFlow.Events;
using TechTalk.SpecFlow.Infrastructure;
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

            _testThreadExecutionEventPublisher.Verify(ep => ep.PublishEvent(It.Is<AttachmentAddedEvent>(m => m.FileName == filePath)), Times.Once);
        }

        private SpecFlowOutputHelper CreateSpecFlowOutputHelper()
        {
            var containerMock = new Mock<IObjectContainer>();
            containerMock.Setup(c => c.ResolveAll<ISpecFlowScenarioOutputListener>()).Returns(Enumerable.Empty<ISpecFlowScenarioOutputListener>());

            _testThreadExecutionEventPublisher = new Mock<ITestThreadExecutionEventPublisher>();

            return new SpecFlowOutputHelper(containerMock.Object, _testThreadExecutionEventPublisher.Object);
        }
    }
}
