using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    
    public class RowExtensionMethodTests_GetEnum
    {
        [Fact]
        public void GetEnum_should_return_the_enum_value_from_the_row()
        {
            var table = new Table("Sex");
            table.AddRow("Male");

            table.Rows.First().GetEnum<Person>("Sex").Should().Be(Sex.Male);
        }

        [Fact]
        public void GetEnum_should_return_the_enum_value_from_the_row_even_when_i_use_spaces()
        {
            var table = new Table("Sex");
            table.AddRow("Unknown Sex");

            table.Rows.First().GetEnum<Person>("Sex").Should().Be(Sex.UnknownSex);
        }

        [Fact]
        public void GetEnum_should_return_the_enum_value_from_the_row_even_when_i_mess_up_the_casing()
        {
            var table = new Table("Sex");
            table.AddRow("feMale");

            table.Rows.First().GetEnum<Person>("Sex").Should().Be(Sex.Female);
        }

        [Fact]
        public void GetEnum_should_return_the_enum_value_from_the_row_even_when_i_mess_up_the_casing_and_use_spaces()
        {
            var table = new Table("Sex");
            table.AddRow("unknown sex");

            table.Rows.First().GetEnum<Person>("Sex").Should().Be(Sex.UnknownSex);
        }

        [Fact]
        public void GetEnum_throws_exception_when_the_value_is_not_defined_in_any_Enum_in_the_type()
        {
            var table = new Table("Sex");
            table.AddRow("NotDefined");

            Action getNotDefinedEnum = () => table.Rows.First().GetEnum<Person>("Sex");

            getNotDefinedEnum.Should().Throw<InvalidOperationException>().WithMessage("No enum with value NotDefined found in type Person");
        }

        [Fact]
        public void GetDiscreteEnum_should_return_enum_of_my_specified_type()
        {
            var table = new Table("Header Not Representing Property Of Any Class");
            table.AddRow("Red");

            var discreteEnum = table.Rows[0].GetDiscreteEnum<Colors>("Header Not Representing Property Of Any Class");

            discreteEnum.Should().Be(Colors.Red);
        }

        [Fact]
        public void GetDiscreteEnum_throws_exception_when_the_value_is_not_defined_in_enum()
        {
            var table = new Table("Header Not Representing Property Of Any Class");
            table.AddRow("NotDefined");

            Action getDiscreteEnum = () => table.Rows[0].GetDiscreteEnum<Colors>("Header Not Representing Property Of Any Class");

            getDiscreteEnum.Should().Throw<InvalidOperationException>().WithMessage("No enum with value NotDefined found in enum Colors");
        }

        [Fact]
        public void GetDiscreteEnum_should_return_not_default_enum_if_value_matched()
        {
            var table = new Table("Header Not Representing Property Of Any Class");
            table.AddRow("Red");

            var discreteEnum = table.Rows[0].GetDiscreteEnum("Header Not Representing Property Of Any Class", Colors.Green);

            discreteEnum.Should().Be(Colors.Red);
        }

        [Fact]
        public void GetDiscreteEnum_should_return_default_enum_if_value_is_not_defined_in_enum()
        {
            var table = new Table("Header Not Representing Property Of Any Class");
            table.AddRow("NotDefined");

            var discreteEnum = table.Rows[0].GetDiscreteEnum("Header Not Representing Property Of Any Class", Colors.Green);

            discreteEnum.Should().Be(Colors.Green);
        }

        [Fact]
        public void GetEnum_should_work_when_there_are_two_enums_of_the_same_type()
        {
            var table = new Table("Color", "AnotherColor");
            table.AddRow("Red", "Green");

            var row = table.Rows.First();

            row.GetEnum<AClassWithTwoEnumsOfTheSameType>("Color")
                .Should().Be(Colors.Red);

            row.GetEnum<AClassWithTwoEnumsOfTheSameType>("AnotherColor")
                .Should().Be(Colors.Green);
        }

        [Fact]
        public void GetEnums_should_only_return_the_enum_of_the_requested_property()
        {
            var table = new Table("ColorsAgain1", "Color", "ColorsAgain2");
            table.AddRow("Green", "Green", "Green");

            var row = table.Rows.First();

            row.GetEnum<AClassWithSimilarEnums>("ColorsAgain1")
                .Should().Be(ColorsAgain.Green);

            row.GetEnum<AClassWithSimilarEnums>("Color")
                .Should().Be(Colors.Green);

            row.GetEnum<AClassWithSimilarEnums>("ColorsAgain2")
                .Should().Be(ColorsAgain.Green);
        }

        public class AClassWithTwoEnumsOfTheSameType
        {
            public Colors Color { get; set; }
            public Colors AnotherColor { get; set; }
        }

        public class AClassWithSimilarEnums
        {
            public ColorsAgain ColorsAgain1 { get; set; }
            public Colors Color { get; set; }
            public ColorsAgain ColorsAgain2 { get; set; }
        }

        public enum Colors { Red, Green, Blue }
        public enum ColorsAgain { Red, Green, Blue}

        public enum ShirtSizes { Small, Medium, Large }
    }
}
