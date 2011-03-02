using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    [TestFixture]
    public class FillInstanceHelperMethodTests : CreateInstanceHelperMethodTestBase
    {
        public FillInstanceHelperMethodTests()
            : base(t =>
                       {
                           var person = new Person();
                           t.FillInstance(person);
                           return person;
                       })
        {
        }
    }
}