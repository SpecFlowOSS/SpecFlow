using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.xUnit.SpecFlowPlugin
{
    public class XUnitAttachmentHandler : ISpecFlowAttachmentHandler
    {
        private readonly ITraceListener _traceListener;

        public XUnitAttachmentHandler(ITraceListener traceListener)
        {
            _traceListener = traceListener;
        }

        public void AddAttachment(string filePath)
        {
            _traceListener.WriteTestOutput($"Attachment added: {filePath}");
        }
    }
}
