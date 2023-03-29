using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class SpecFlowAttachmentHandler : ISpecFlowAttachmentHandler
    {
        private readonly ITraceListener _traceListener;

        protected SpecFlowAttachmentHandler(ITraceListener traceListener)
        {
            _traceListener = traceListener;
        }

        public virtual void AddAttachment(string filePath)
        {
            _traceListener.WriteToolOutput($"Attachment '{filePath}' added (not forwarded to the test runner).");
        }
    }
}
