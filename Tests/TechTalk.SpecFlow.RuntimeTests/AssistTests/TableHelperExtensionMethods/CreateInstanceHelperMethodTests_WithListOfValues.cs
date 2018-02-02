using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    class CreateInstanceHelperMethodTests_WithListOfValues : CreateInstanceHelperMethodTestBase
    {
        public CreateInstanceHelperMethodTests_WithListOfValues()
            : base(t => t.CreateInstance<Person>())
        {
        }

        [Test]
        public void Can_create_an_instance_with_list_of_strings_from_comma_separated_list_of_strings()
        {
            const string first = "First string";
            const string second = "Second string";

            var table = new Table("Field", "Value");
            table.AddRow("StringList", $"{first}, {second}");
 
            var @class = table.CreateInstance<Person>();

            @class.StringList[0].Should().Be(first);
            @class.StringList[1].Should().Be(second);
        }
    }
}
