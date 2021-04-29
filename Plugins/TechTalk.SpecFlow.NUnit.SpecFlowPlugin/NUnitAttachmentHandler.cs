using NUnit.Framework;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.NUnit.SpecFlowPlugin
{
    public class NUnitAttachmentHandler : ISpecFlowAttachmentHandler
    {
        public void AddAttachment(string filePath)
        {
            TestContext.AddTestAttachment(filePath);
        }
    }
}
