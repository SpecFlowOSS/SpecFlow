using System;
using BoDi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.MSTest.SpecFlowPlugin
{
    public class MSTestAttachmentHandler : SpecFlowAttachmentHandler
    {
        private readonly IMSTestTestContextProvider _testContextProvider;

        public MSTestAttachmentHandler(ITraceListener traceListener, IMSTestTestContextProvider testContextProvider) : base(traceListener)
        {
            _testContextProvider = testContextProvider;
        }

        public override void AddAttachment(string filePath)
        {
            try
            {
                _testContextProvider.GetTestContext().AddResultFile(filePath);
            }
            catch (Exception)
            {
                base.AddAttachment(filePath);
            }
        }
    }
}
