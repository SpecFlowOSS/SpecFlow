using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    [TestFixture]
    public class CreateInstanceHelperMethodTests : CreateInstanceHelperMethodTestBase
    {
        public CreateInstanceHelperMethodTests()
            : base(t => t.CreateInstance<Person>())
        {
        }
    }
}