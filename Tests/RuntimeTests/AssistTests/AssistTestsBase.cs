using NUnit.Framework;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    public class AssistTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            ScenarioContext.Current = new ScenarioContext(null, null, null);
        }
    }
}