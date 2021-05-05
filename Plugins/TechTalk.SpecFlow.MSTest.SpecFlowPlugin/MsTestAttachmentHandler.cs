using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.MSTest.SpecFlowPlugin
{
    public class MSTestAttachmentHandler : SpecFlowAttachmentHandler
    {
        private readonly TestContext _testContext;

        public MSTestAttachmentHandler(ITraceListener traceListener, TestContext testContext) : base(traceListener)
        {
            _testContext = testContext;
        }

        public override void AddAttachment(string filePath)
        {
            try
            {
                _testContext.AddResultFile(filePath);
            }
            catch (Exception)
            {
                base.AddAttachment(filePath);
            }
        }
    }
}
