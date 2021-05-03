using System;
using NUnit.Framework;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.NUnit.SpecFlowPlugin
{
    public class NUnitAttachmentHandler : SpecFlowAttachmentHandler
    {
        public NUnitAttachmentHandler(ITraceListener traceListener) : base(traceListener)
        {
        }

        public override void AddAttachment(string filePath)
        {
            try
            {
                TestContext.AddTestAttachment(filePath);
            }
            catch (Exception)
            {
                base.AddAttachment(filePath);
            }
        }
    }
}
