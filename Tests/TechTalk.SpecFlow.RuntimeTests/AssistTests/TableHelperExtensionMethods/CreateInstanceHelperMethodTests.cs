using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    
    public class CreateInstanceHelperMethodTests : CreateInstanceHelperMethodTestBase
    {
        public CreateInstanceHelperMethodTests()
            : base(t => t.CreateInstance<Person>())
        {
        }

        [Fact]
        public virtual void Create_instance_will_return_an_instance_of_T()
        {
            var table = new Table("Field", "Value");
            var person = GetThePerson(table);
            person.Should().NotBeNull();
        }

        [Fact]
        public void Can_create_an_instance_with_similar_enum_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("FourthColor", "Green");
            table.AddRow("ThirdColor", "Red");
            table.AddRow("SecondColor", "Red");
            table.AddRow("FirstColor", "Red");

            var @class = table.CreateInstance<AClassWithMultipleEnums>();

            @class.FirstColor.Should().Be(AClassWithMultipleEnums.Color.Red);
            @class.SecondColor.Should().Be(AClassWithMultipleEnums.ColorAgain.Red);
            @class.ThirdColor.Should().Be(AClassWithMultipleEnums.Color.Red);
            @class.FourthColor.Should().Be(AClassWithMultipleEnums.ColorAgain.Green);
        }

        [Fact]
        public void Can_create_an_instance_with_a_constructor()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Field2", "Entry2");
            table.AddRow("Field1", "Entry1");

            var @class = table.CreateInstance<AClassWithAConstructor>();

            @class.Field1.Should().Be("Entry1");
            @class.Field2.Should().Be("Entry2");
        }

        [Fact]
        public void Can_create_an_instance_with_a_constructor_with_default_parameters()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Field1", "Entry1");

            var @class = table.CreateInstance<AClassWithAConstructorWithDefaultParameters>();

            @class.Field1.Should().Be("Entry1");
            @class.Field2.Should().BeNull();
            @class.Field3.Should().Be("Value3");
        }

        public class AClassWithMultipleEnums
        {
            public Color FirstColor { get; set; }
            public ColorAgain SecondColor { get; set; }
            public Color ThirdColor { get; set; }
            public ColorAgain FourthColor { get; set; }

            public enum Color { Red, Green, Blue }
            public enum ColorAgain { Red, Green, Blue}
        }

        public class AClassWithAConstructor
        {
            public AClassWithAConstructor(string field1, string field2)
            {
                Field1 = field1;
                Field2 = field2;
            }

            public string Field1 { get; }
            public string Field2 { get; }
        }

        public class AClassWithAConstructorWithDefaultParameters
        {
            public AClassWithAConstructorWithDefaultParameters(string field1, string field2, string field3 = "Value3")
            {
                Field1 = field1;
                Field2 = field2;
                Field3 = field3;
            }

            public string Field1 { get; }
            public string Field2 { get; }
            public string Field3 { get; }
        }
    }
}