using TechTalk.SpecFlow.Events;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class SpecFlowOutputHelper : ISpecFlowOutputHelper
    {
        private readonly ITestThreadExecutionEventPublisher _testThreadExecutionEventPublisher;
        private readonly ITraceListener _traceListener;
        private readonly ISpecFlowAttachmentHandler _specFlowAttachmentHandler;

        public SpecFlowOutputHelper(ITestThreadExecutionEventPublisher testThreadExecutionEventPublisher, ITraceListener traceListener, ISpecFlowAttachmentHandler specFlowAttachmentHandler)
        {
            _testThreadExecutionEventPublisher = testThreadExecutionEventPublisher;
            _traceListener = traceListener;
            _specFlowAttachmentHandler = specFlowAttachmentHandler;
        }

        public void WriteLine(string message)
        {
            _testThreadExecutionEventPublisher.PublishEvent(new OutputAddedEvent(message));
            _traceListener.WriteToolOutput(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        public void AddAttachment(string filePath)
        {
            _testThreadExecutionEventPublisher.PublishEvent(new AttachmentAddedEvent(filePath));
            _specFlowAttachmentHandler.AddAttachment(filePath);
        }
    }
}
