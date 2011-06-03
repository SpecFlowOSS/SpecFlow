using NUnit.Framework;
using Should;
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

        [Test]
        public virtual void Can_use_horizontal_tables_with_FillInstance()
        {
            var table = new Table("MiddleInitial", "NullableChar");
            table.AddRow("T", "S");

            var person = GetThePerson(table);

            person.MiddleInitial.ShouldEqual('T');
            person.NullableChar.ShouldEqual('S');
        }
    }
}