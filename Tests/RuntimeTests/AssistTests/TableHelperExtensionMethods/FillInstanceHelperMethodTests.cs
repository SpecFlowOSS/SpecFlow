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

        [Test]
        public virtual void Can_populate_subtype_fields_with_FillInstance()
        {
            Person person = new PersonWithStyle();

            var table = new Table("Property", "Value");
            table.AddRow("Style", "VeryCool");

            table.FillInstance(person);

            // Create a variable of type PersonWithStyle to validate that Style was set properly.
            PersonWithStyle personWithStyle = (PersonWithStyle)person;

            Assert.That(personWithStyle.Style, Is.EqualTo(Style.VeryCool));
        }
    }
}