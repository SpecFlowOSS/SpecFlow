using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    public class CreateInstanceHelperMethodTests_WithArrayOfValues
    {

        [Fact]
        public void Can_create_an_instance_with_string_array_from_comma_separated_list_of_strings()
        {
            const string first = "First string";
            const string second = "Second string";

            var table = new Table("Field", "Value");
            table.AddRow("StringArray", $"{first}, {second}");
 
            var @class = table.CreateInstance<Person>();

            @class.StringArray[0].Should().Be(first);
            @class.StringArray[1].Should().Be(second);
            @class.StringArray.Length.Should().Be(2);
        }
    }
}
