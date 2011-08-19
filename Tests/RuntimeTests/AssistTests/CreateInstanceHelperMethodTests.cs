using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class CreateInstanceHelperMethodTests
    {
        [SetUp]
        public void TestSetup()
        {
            // this is required, because the tests depend on parsing decimals with the en-US culture
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        [Test]
        public void Create_instance_will_return_an_instance_of_T()
        {
            var table = new Table("Field", "Value");
            var person = table.CreateInstance<Person>();
            person.ShouldNotBeNull();
        }

        [Test]
        public void Create_instance_will_set_values_with_a_vertical_table_when_there_is_one_row_and_one_column()
        {
            var table = new Table("FirstName");
            table.AddRow("Howard");
            var person = table.CreateInstance<Person>();

            person.FirstName.ShouldEqual("Howard");
        }

        [Test]
        public void When_one_row_exists_with_two_headers_and_the_first_row_value_is_not_a_property_then_treat_as_horizontal_table()
        {
            var table = new Table("AnotherValue", "YetAnotherValue");
            table.AddRow("x", "y");

            var test = table.CreateInstance<TestingVerticalTable>();
            test.AnotherValue.ShouldEqual("x");
            test.YetAnotherValue.ShouldEqual("y");
        }

        [Test]
        public void When_one_row_exists_with_two_headers_and_the_first_row_value_is_a_property_then_treat_as_a_vertical_table()
        {
            var table = new Table("AnotherValue", "YetAnotherValue");
            table.AddRow("AnotherValue", "applesauce");

            var test = table.CreateInstance<TestingVerticalTable>();
            test.AnotherValue.ShouldEqual("applesauce");
            test.YetAnotherValue.ShouldBeNull();
        }

        [Test]
        public void When_one_row_exists_with_two_headers_and_the_first_row_value_is_a_property_without_perfect_name_match_then_treat_as_a_vertical_table()
        {
            var table = new Table("AnotherValue", "YetAnotherValue");
            table.AddRow("Another  value", "applesauce");

            var test = table.CreateInstance<TestingVerticalTable>();
            test.AnotherValue.ShouldEqual("applesauce");
            test.YetAnotherValue.ShouldBeNull();
        }

        [Test]
        public void When_one_row_exists_with_three_headers_then_treat_as_horizontal_table()
        {
            var table = new Table("Field", "Value", "AnotherValue");
            table.AddRow("one", "two", "three");

            var test = table.CreateInstance<TestingVerticalTable>();
            test.Field.ShouldEqual("one");
            test.Value.ShouldEqual("two");
            test.AnotherValue.ShouldEqual("three");
        }

        public class TestingVerticalTable
        {
            public string Field { get; set; }
            public string Value { get; set; }
            public string AnotherValue { get; set; }
            public string YetAnotherValue { get; set; }
        }

        [Test]
        public void Sets_string_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("FirstName", "John");
            table.AddRow("LastName", "Galt");

            var person = table.CreateInstance<Person>();

            person.FirstName.ShouldEqual("John");
            person.LastName.ShouldEqual("Galt");
        }

        [Test]
        public void Sets_int_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NumberOfIdeas", "3");

            var person = table.CreateInstance<Person>();

            person.NumberOfIdeas.ShouldEqual(3);
        }

        [Test]
        public void Sets_nullable_int_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableInt", "9");

            var person = table.CreateInstance<Person>();

            person.NullableInt.ShouldEqual(9);
        }

        [Test]
        public void Sets_uint_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("UnsignedInt", "3");

            var person = table.CreateInstance<Person>();

            person.UnsignedInt.ShouldEqual<uint>(3);
        }

        [Test]
        public void Sets_nullable_uint_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableUnsignedInt", "9");

            var person = table.CreateInstance<Person>();

            person.NullableUnsignedInt.ShouldEqual<uint?>(9);
        }

        [Test]
        public void Sets_decimal_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Salary", "9.78");

            var person = table.CreateInstance<Person>();

            person.Salary.ShouldEqual(9.78M);
        }

        [Test]
        public void Sets_nullable_decimal_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableDecimal", "19.78");

            var person = table.CreateInstance<Person>();

            person.NullableDecimal.ShouldEqual(19.78M);
        }

        [Test]
        public void Sets_bool_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("IsRational", "true");

            var person = table.CreateInstance<Person>();

            person.IsRational.ShouldBeTrue();
        }

        [Test]
        public void Sets_nullable_bool_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableBool", "true");

            var person = table.CreateInstance<Person>();

            person.NullableBool.ShouldEqual(true);
        }

        [Test]
        public void Sets_datetime_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("BirthDate", "12/31/2010");

            var person = table.CreateInstance<Person>();

            person.BirthDate.ShouldEqual(new DateTime(2010, 12, 31));
        }

        [Test]
        public void Sets_nullable_datetime_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableDateTime", "11/30/2010");

            var person = table.CreateInstance<Person>();

            person.NullableDateTime.ShouldEqual(new DateTime(2010, 11, 30));
        }

        [Test]
        public void Sets_double_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Double", "4.235");

            var person = table.CreateInstance<Person>();

            person.Double.ShouldEqual(4.235);
        }

        [Test]
        public void Sets_nullable_double_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableDouble", "7.218");

            var person = table.CreateInstance<Person>();

            person.NullableDouble.ShouldEqual(7.218);
        }

        [Test]
        public void Sets_Guid_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("GuidId", "B48D8AF4-405F-4286-B83E-774EA773CFA3");

            var person = table.CreateInstance<Person>();

            person.GuidId.ShouldEqual(new Guid("B48D8AF4-405F-4286-B83E-774EA773CFA3"));
        }

        [Test]
        public void Sets_nullable_guid_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableGuidId", "B48D8AF4-405F-4286-B83E-774EA773CFA3");

            var person = table.CreateInstance<Person>();

            person.NullableGuidId.ShouldEqual(new Guid("B48D8AF4-405F-4286-B83E-774EA773CFA3"));
        }

        [Test]
        public void Sets_float_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Float", "98.22");

            var person = table.CreateInstance<Person>();

            person.Float.ShouldEqual(98.22F);
        }

        [Test]
        public void Sets_nullable_float_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableFloat", "55.66");

            var person = table.CreateInstance<Person>();

            person.NullableFloat.ShouldEqual(55.66F);
        }

        [Test]
        public void Ignores_spaces_when_matching_enum_property_name()
        {
            var table = new Table("Field", "Value");
            table.AddRow("This Is A Style", "Soft");

            var test = table.CreateInstance<EnumPropertyTest>();

            test.ThisIsAStyle.ShouldEqual(Style.Soft);
        }

        [Test]
        public void Ignores_casing_when_matching_enum_property_name()
        {
            var table = new Table("Field", "Value");
            table.AddRow("ThisisaSTYLE", "VeryCool");

            var test = table.CreateInstance<EnumPropertyTest>();

            test.ThisIsAStyle.ShouldEqual(Style.VeryCool);
        }

        private class EnumPropertyTest
        {
            public Style ThisIsAStyle { get; set; }
        }



    }
}
