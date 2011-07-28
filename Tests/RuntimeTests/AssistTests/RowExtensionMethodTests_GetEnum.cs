using System;
using System.Linq;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;
using RowExtensionMethods = TechTalk.SpecFlow.Assist.RowExtensionMethods;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class RowExtensionMethodTests_GetEnum
    {
        [Test]
        public void GetEnum_should_return_the_enum_value_from_the_row()
        {
            var table = new Table("Sex");
            table.AddRow("Male");

            ObjectAssertExtensions.ShouldEqual<Enum>(table.Rows.First()
                                                              .GetEnum<Person>("Sex"), Sex.Male);
        }

        [Test]
        public void GetEnum_should_return_the_enum_value_from_the_row_even_when_i_use_spaces()
        {
            var table = new Table("Sex");
            table.AddRow("Unknown Sex");

            ObjectAssertExtensions.ShouldEqual<Enum>(table.Rows.First()
                                                              .GetEnum<Person>("Sex"), Sex.UnknownSex);
        }

        [Test]
        public void GetEnum_should_return_the_enum_value_from_the_row_even_when_i_mess_up_the_casing()
        {
            var table = new Table("Sex");
            table.AddRow("feMale");

            ObjectAssertExtensions.ShouldEqual<Enum>(table.Rows.First()
                                                              .GetEnum<Person>("Sex"), Sex.Female);
        }

        [Test]
        public void GetEnum_should_return_the_enum_value_from_the_row_even_when_i_mess_up_the_casing_and_use_spaces()
        {
            var table = new Table("Sex");
            table.AddRow("unknown sex");

            ObjectAssertExtensions.ShouldEqual<Enum>(table.Rows.First()
                                                              .GetEnum<Person>("Sex"), Sex.UnknownSex);
        }

        [Test]
        public void GetEnum_throws_exception_when_the_value_is_not_defined_in_any_Enum_in_the_type()
        {
            var table = new Table("Sex");
            table.AddRow("NotDefinied");

            var exceptionThrown = false;
            try
            {
                var e = RowExtensionMethods.GetEnum<Person>(table.Rows.First(), "Sex");
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message == "No enum with value NotDefinied found in type Person")
                    exceptionThrown = true;
            }
            exceptionThrown.ShouldBeTrue();
        }

        [Test]
        public void GetEnum_should_work_when_there_are_two_enums_of_the_same_type()
        {
            var table = new Table("Color", "AnotherColor");
            table.AddRow("Red", "Green");

            var row = table.Rows.First();

            row.GetEnum<AClassWithTwoEnumsOfTheSameType>("Color")
                .ShouldEqual(Colors.Red);

            row.GetEnum<AClassWithTwoEnumsOfTheSameType>("AnotherColor")
                .ShouldEqual(Colors.Green);
        }

        [Test]
        public void GetEnums_should_only_return_the_enum_of_the_requested_property()
        {
            var table = new Table("ColorsAgain1", "Color", "ColorsAgain2");
            table.AddRow("Green", "Green", "Green");

            var row = table.Rows.First();

            row.GetEnum<AClassWithSimilarEnums>("ColorsAgain1")
                .ShouldEqual(ColorsAgain.Green);

            row.GetEnum<AClassWithSimilarEnums>("Color")
                .ShouldEqual(Colors.Green);

            row.GetEnum<AClassWithSimilarEnums>("ColorsAgain2")
                .ShouldEqual(ColorsAgain.Green);
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