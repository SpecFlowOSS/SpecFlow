using System;
using System.Globalization;
using System.Threading;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.Attributes;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;


namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{

    public class CreateInstanceHelperMethodTests
    {
        public CreateInstanceHelperMethodTests()
        {
            // this is required, because the tests depend on parsing decimals with the en-US culture
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

        }

        [Fact]
        public void Create_instance_will_return_an_instance_of_T()
        {
            var table = new Table("Field", "Value");
            var person = table.CreateInstance<Person>();
            person.Should().NotBeNull();
        }

        [Fact]
        public void Create_instance_will_set_values_with_a_vertical_table_when_there_is_one_row_and_one_column()
        {
            var table = new Table("FirstName");
            table.AddRow("Howard");
            var person = table.CreateInstance<Person>();

            person.FirstName.Should().Be("Howard");
        }

        [Fact]
        public void Create_instance_will_set_values_with_a_vertical_table_and_unbound_column_throws_ColumnCouldNotBeBoundException_on_verify()
        {
            var table = new Table("FirstNaame");
            table.AddRow("Howard");

            Action act = () => table.CreateInstance<Person>(new InstanceCreationOptions() { VerifyAllColumnsBound = true });
            act.Should().Throw<ColumnCouldNotBeBoundException>();
        }

        [Fact]
        public void When_one_row_exists_with_two_headers_and_the_first_row_value_is_not_a_property_then_treat_as_horizontal_table()
        {
            var table = new Table("AnotherValue", "YetAnotherValue");
            table.AddRow("x", "y");

            var test = table.CreateInstance<TestingVerticalTable>();
            test.AnotherValue.Should().Be("x");
            test.YetAnotherValue.Should().Be("y");
        }

        [Fact]
        public void When_one_row_exists_with_two_headers_and_the_first_row_value_is_a_property_then_treat_as_a_vertical_table()
        {
            var table = new Table("AnotherValue", "YetAnotherValue");
            table.AddRow("AnotherValue", "applesauce");

            var test = table.CreateInstance<TestingVerticalTable>();
            test.AnotherValue.Should().Be("applesauce");
            test.YetAnotherValue.Should().Be(null);
        }

        [Fact]
        public void When_one_row_exists_with_two_headers_and_the_first_row_value_is_a_property_without_perfect_name_match_then_treat_as_a_vertical_table()
        {
            var table = new Table("AnotherValue", "YetAnotherValue");
            table.AddRow("Another  value", "applesauce");

            var test = table.CreateInstance<TestingVerticalTable>();
            test.AnotherValue.Should().Be("applesauce");
            test.YetAnotherValue.Should().Be(null);
        }

        [Fact]
        public void When_one_row_exists_with_three_headers_then_treat_as_horizontal_table()
        {
            var table = new Table("Field", "Value", "AnotherValue");
            table.AddRow("one", "two", "three");

            var test = table.CreateInstance<TestingVerticalTable>();
            test.Field.Should().Be("one");
            test.Value.Should().Be("two");
            test.AnotherValue.Should().Be("three");
        }

        public class TestingVerticalTable
        {
            public string Field { get; set; }
            public string Value { get; set; }
            public string AnotherValue { get; set; }
            public string YetAnotherValue { get; set; }
        }

        [Fact]
        public void Sets_string_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("FirstName", "John");
            table.AddRow("LastName", "Galt");

            var person = table.CreateInstance<Person>();

            person.FirstName.Should().Be("John");
            person.LastName.Should().Be("Galt");
        }

        [Fact]
        public void Sets_string_values_unbound_column_throws_ColumnCouldNotBeBoundException_on_verify()
        {
            var table = new Table("Field", "Value");
            table.AddRow("FirstNaame", "John");
            table.AddRow("LastName", "Galt");

            Action act = () => table.CreateInstance<Person>(new InstanceCreationOptions { VerifyAllColumnsBound = true });
            act.Should().Throw<ColumnCouldNotBeBoundException>();
        }

        [Fact]
        public void SetConstructor_unbound_column_throws_ColumnCouldNotBeBoundException_on_verify()
        {
            var table = new Table("Field", "Value");
            table.AddRow("FirstNaame", "John");
            table.AddRow("LastName", "Galt");

            Action act = () => table.CreateInstance<PersonWithMandatoryLastName>(new InstanceCreationOptions { VerifyAllColumnsBound = true });
            act.Should().Throw<ColumnCouldNotBeBoundException>();
        }

        [Fact]
        public void Sets_int_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NumberOfIdeas", "3");

            var person = table.CreateInstance<Person>();

            person.NumberOfIdeas.Should().Be(3);
        }

        [Fact]
        public void Sets_nullable_int_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableInt", "9");

            var person = table.CreateInstance<Person>();

            person.NullableInt.Should().Be(9);
        }

        [Fact]
        public void Sets_uint_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("UnsignedInt", "3");

            var person = table.CreateInstance<Person>();

            person.UnsignedInt.Should().Be(3);
        }

        [Fact]
        public void Sets_nullable_uint_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableUnsignedInt", "9");

            var person = table.CreateInstance<Person>();

            person.NullableUnsignedInt.Should().Be((uint?)9);
        }

        [Fact]
        public void Sets_decimal_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Salary", "9.78");

            var person = table.CreateInstance<Person>();

            person.Salary.Should().Be(9.78M);
        }

        [Fact]
        public void Sets_nullable_decimal_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableDecimal", "19.78");

            var person = table.CreateInstance<Person>();

            person.NullableDecimal.Should().Be(19.78M);
        }

        [Fact]
        public void Sets_bool_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("IsRational", "true");

            var person = table.CreateInstance<Person>();

            person.IsRational.Should().BeTrue();
        }

        [Fact]
        public void Sets_nullable_bool_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableBool", "true");

            var person = table.CreateInstance<Person>();

            person.NullableBool.Should().Be(true);
        }

        [Fact]
        public void Sets_datetime_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("BirthDate", "12/31/2010");

            var person = table.CreateInstance<Person>();

            person.BirthDate.Should().Be(new DateTime(2010, 12, 31));
        }

        [Fact]
        public void Sets_nullable_datetime_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableDateTime", "11/30/2010");

            var person = table.CreateInstance<Person>();

            person.NullableDateTime.Should().Be(new DateTime(2010, 11, 30));
        }

        [Fact]
        public void Sets_double_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Double", "4.235");

            var person = table.CreateInstance<Person>();

            person.Double.Should().Be(4.235);
        }

        [Fact]
        public void Sets_nullable_double_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableDouble", "7.218");

            var person = table.CreateInstance<Person>();

            person.NullableDouble.Should().Be(7.218);
        }

        [Fact]
        public void Sets_Guid_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("GuidId", "B48D8AF4-405F-4286-B83E-774EA773CFA3");

            var person = table.CreateInstance<Person>();

            person.GuidId.Should().Be(new Guid("B48D8AF4-405F-4286-B83E-774EA773CFA3"));
        }

        [Fact]
        public void Sets_nullable_guid_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableGuidId", "B48D8AF4-405F-4286-B83E-774EA773CFA3");

            var person = table.CreateInstance<Person>();

            person.NullableGuidId.Should().Be(new Guid("B48D8AF4-405F-4286-B83E-774EA773CFA3"));
        }

        [Fact]
        public void Sets_float_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Float", "98.22");

            var person = table.CreateInstance<Person>();

            person.Float.Should().Be(98.22F);
        }

        [Fact]
        public void Sets_nullable_float_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NullableFloat", "55.66");

            var person = table.CreateInstance<Person>();

            person.NullableFloat.Should().Be(55.66F);
        }

        [Fact]
        public void Ignores_spaces_when_matching_enum_property_name()
        {
            var table = new Table("Field", "Value");
            table.AddRow("This Is A Style", "Soft");

            var test = table.CreateInstance<EnumPropertyTest>();

            test.ThisIsAStyle.Should().Be(Style.Soft);
        }

        [Fact]
        public void Ignores_casing_when_matching_enum_property_name()
        {
            var table = new Table("Field", "Value");
            table.AddRow("ThisisaSTYLE", "VeryCool");

            var test = table.CreateInstance<EnumPropertyTest>();

            test.ThisIsAStyle.Should().Be(Style.VeryCool);
        }

        private class EnumPropertyTest
        {
            public Style ThisIsAStyle { get; set; }
        }

        [Fact]
        public void Replaces_special_characters_when_matching_property_names()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Prop.1", "hello");
            table.AddRow("Prop.2", "world");

            var test = table.CreateInstance<Prop>();

            test.Prop1.Should().Be("hello");
            test.Prop2.Should().Be("world");
        }

        [Fact]
        public void Works_with_snake_case()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Look at me", "hello");
            table.AddRow("This is so long", "world");

            var test = table.CreateInstance<Snake>();

            test.Look_at_me.Should().Be("hello");
            test.this_is_so_long.Should().Be("world");
        }

        [Fact]
        public void Works_with_tuples()
        {
            var table = new Table("PropertyOne", "PropertyTwo", "PropertyThree");
            table.AddRow("Look at me", "hello", "999");

            var test = table.CreateInstance<(string one, string two, int three)>();

            test.one.Should().Be("Look at me");
            test.two.Should().Be("hello");
            test.three.Should().Be(999);
        }

        [Fact]
        public void Too_long_tuples_throw_exception()
        {
            var table = new Table("PropertyOne", "PropertyTwo", "PropertyThree", "PropertyFour", "PropertyFive", "PropertySix", "PropertySeven", "PropertyEight");
            table.AddRow("Look at me", "hello", "999", "this", "should", "actually", "fail", "right?");

            Assert.Throws<Exception>(() => table.CreateInstance<(string one, string two, int three, string four, string five, string six, string seven, string eight)>());
        }

        [Fact]
        public void Works_with_tuples_vertical_format()
        {
            var table = new Table("Field", "Value");
            table.AddRow("One", "hello");
            table.AddRow("Two", "world");
            table.AddRow("Three", "999");

            var test = table.CreateInstance<(string one, string two, int three)>();

            test.one.Should().Be("hello");
            test.two.Should().Be("world");
            test.three.Should().Be(999);
        }

        [Fact]
        public void Uses_property_aliases()
        {
            var table = new Table("AliasOne", "AliasTwo", "AliasThree");
            table.AddRow("PropertyOne", "PropertyTwo", "PropertyThree");

            var test = table.CreateInstance<AliasedClass>();
            test.PropertyOne.Should().Be("PropertyOne");
            test.PropertyTwo.Should().Be("PropertyTwo");
            test.PropertyThree.Should().Be("PropertyThree");
        }

        [Fact]
        public void Uses_field_aliases()
        {
            var table = new Table("FieldAliasOne", "FieldAliasTwo", "FieldAliasThree");
            table.AddRow("FieldOne", "FieldTwo", "FieldThree");

            var test = table.CreateInstance<AliasedClass>();
            test.FieldOne.Should().Be("FieldOne");
            test.FieldTwo.Should().Be("FieldTwo");
            test.FieldThree.Should().Be("FieldThree");
        }

        [Fact]
        public void Property_aliases_allow_multiple_property_population()
        {
            var table = new Table("AliasOne", "AliasTwo", "AliasThree");
            table.AddRow("PropertyOne", "PropertyTwo", "PropertyThree");

            var test = table.CreateInstance<AliasedClass>();
            test.PropertyOne.Should().Be("PropertyOne");
            test.AnotherPropertyWithSameAlias.Should().Be("PropertyOne");
        }

        [Fact]
        public void Property_aliases_do_not_allow_type_mismatch_property_population()
        {
            var table = new Table("AliasOne", "AliasTwo", "AliasThree");
            table.AddRow("PropertyOne", "PropertyTwo", "PropertyThree");

            var test = table.CreateInstance<AliasedClass>();
            test.PropertyOne.Should().Be("PropertyOne");
            test.AliasedButTypeMismatch.Should().Be(0);
        }

        [Fact]
        public void Property_aliases_work_for_vertical_format()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Alias One", "Hello");
            table.AddRow("AliasTwo", "World");
            table.AddRow("AliasThree", "From Rich");

            var test = table.CreateInstance<AliasedClass>();
            test.PropertyOne.Should().Be("Hello");
            test.PropertyTwo.Should().Be("World");
            test.PropertyThree.Should().Be("From Rich");
        }

        [Theory]
        [InlineData("FirstName", "MiddleName", "Surname")]
        [InlineData("FirstName", "MiddleName", "Lastname")]
        [InlineData("First Name", "Middle Name", "Last name")]
        [InlineData("Known As", "Never Known As", "Dad's Last Name")]
        public void Property_can_have_many_aliases_and_uses_regex_to_match_business_jargon(string firstNameAlias, string middleNameAlias, string lastNameAlias)
        {
            var table = new Table("Field", "Value");
            table.AddRow(firstNameAlias, "Richard");
            table.AddRow(middleNameAlias, "David");
            table.AddRow(lastNameAlias, "Linnell");

            var test = table.CreateInstance<AliasedClass>();
            test.PropertyOne.Should().Be("Richard");
            test.PropertyTwo.Should().Be("David");
            test.PropertyThree.Should().Be("Linnell");
        }

        private class Prop
        {
            public string Prop1 { get; set; }
            public string Prop2 { get; set; }
        }

        private class Snake
        {
            public string Look_at_me { get; set; }
            public string this_is_so_long { get; set; }
        }

        private class AliasedClass
        {
            [TableAliases("Alias[ ]*One", "First[ ]?Name", "^Known As$")]
            public string PropertyOne { get; set; }

            [TableAliases("Alias[ ]*Two", "Middle[ ]?Name", "^Never Known As$")]
            public string PropertyTwo { get; set; }

            [TableAliases("AliasThree")]
            [TableAliases("Surname")]
            [TableAliases("Last[ ]?name")]
            [TableAliases("Dad's Last Name")]
            public string PropertyThree { get; set; }

#pragma warning disable 649
            [TableAliases("FieldAliasOne")]
            public string FieldOne;
            [TableAliases("FieldAliasTwo")]
            public string FieldTwo;
            [TableAliases("FieldAliasThree")]
            public string FieldThree;
#pragma warning restore 649

            [TableAliases("AliasOne")]
            public string AnotherPropertyWithSameAlias { get; set; }

            [TableAliases("AliasOne")]
            public long AliasedButTypeMismatch { get; set; }
        }
    }
}
