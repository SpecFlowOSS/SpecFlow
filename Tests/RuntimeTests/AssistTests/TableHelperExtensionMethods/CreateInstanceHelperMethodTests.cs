using FluentAssertions;
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

        [Test]
        public virtual void Create_instance_will_return_an_instance_of_T()
        {
            var table = new Table("Field", "Value");
            var person = GetThePerson(table);
            person.Should().NotBeNull();
        }

        [Test]
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

        public class AClassWithMultipleEnums
        {
            public Color FirstColor { get; set; }
            public ColorAgain SecondColor { get; set; }
            public Color ThirdColor { get; set; }
            public ColorAgain FourthColor { get; set; }

            public enum Color { Red, Green, Blue }
            public enum ColorAgain { Red, Green, Blue}
        }
    }
}