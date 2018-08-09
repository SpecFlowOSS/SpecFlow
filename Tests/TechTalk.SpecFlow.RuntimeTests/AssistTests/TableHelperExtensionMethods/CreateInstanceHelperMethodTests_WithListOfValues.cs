using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    public class CreateInstanceHelperMethodTests_WithListOfValues
    {
        [Fact]
        public void Can_create_an_instance_with_list_of_strings_from_comma_separated_list_of_strings()
        {
            const string first = "First string";
            const string second = "Second string";

            var table = new Table("Field", "Value");
            table.AddRow("StringList", $"{first}, {second}");
 
            var @class = table.CreateInstance<Person>();

            @class.StringList[0].Should().Be(first);
            @class.StringList[1].Should().Be(second);
            @class.StringList.Count.Should().Be(2);
        }
    }
}
