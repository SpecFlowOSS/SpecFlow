using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    
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

        [Fact]
        public virtual void Can_use_horizontal_tables_with_FillInstance()
        {
            var table = new Table("MiddleInitial", "NullableChar");
            table.AddRow("T", "S");

            var person = GetThePerson(table);

            person.MiddleInitial.Should().Be('T');
            person.NullableChar.Should().Be('S');
        }

        [Fact]
        public virtual void Can_populate_subtype_properties_with_FillInstance()
        {
            Person person = new PersonWithStyle();

            var table = new Table("Property", "Value");
            table.AddRow("Style", "VeryCool");

            table.FillInstance(person);

            // Create a variable of type PersonWithStyle to validate that Style was set properly.
            PersonWithStyle personWithStyle = (PersonWithStyle)person;

            personWithStyle.Style.Should().Be(Style.VeryCool);
        }

        [Fact]
        public virtual void Can_populate_properties_and_fields()
        {
            var entity = new EntityWithPropertiesAndFields();

            var table = new Table("Member", "Value");
            table.AddRow("Property1", "Property1");
            table.AddRow("Property2", "100");
            table.AddRow("Field1", "Field1");
            table.AddRow("Field2", "100");

            table.FillInstance(entity);

            entity.Property1.Should().Be("Property1");
            entity.Property2.Should().Be(100);

            entity.Field1.Should().Be("Field1");
            entity.Field2.Should().Be(100);
        }
    }
}