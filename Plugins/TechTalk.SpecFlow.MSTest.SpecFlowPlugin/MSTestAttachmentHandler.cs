using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.MSTest.SpecFlowPlugin
{
    public class MSTestAttachmentHandler : ISpecFlowAttachmentHandler
    {
        private readonly TestContext _testContext;

        public MSTestAttachmentHandler(TestContext testContext)
        {
            _testContext = testContext;
        }

        public void AddAttachment(string filePath)
        {
            // forward to test runner
        }
    }
}
