using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using FluentAssertions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    [TestFixture]
    public class CreateSetHelperMethodTests
    {
        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        private static Table CreatePersonTableHeaders()
        {
            return new Table("FirstName", "LastName", "BirthDate", "NumberOfIdeas", "Salary", "IsRational");
        }

        [Test]
        public void Returns_empty_set_of_type_when_there_are_no_rows()
        {
            var table = new Table("FirstName");
            var people = table.CreateSet<Person>();
            people.Count().Should().Be(0);
        }

        [Test]
        public void Returns_one_instance_when_there_is_one_row()
        {
            var table = new Table("FirstName");
            table.AddRow("John");
            var people = table.CreateSet<Person>();
            people.Count().Should().Be(1);
        }

        [Test]
        public void Sets_properties_with_different_case()
        {
            var table = new Table("firstname");
            table.AddRow("John");
            var people = table.CreateSet<Person>();
            people.First().FirstName.Should().Be("John");
        }

        [Test]
        public void Sets_properties_from_column_names_with_blanks()
        {
            var table = new Table("first name");
            table.AddRow("John");
            var people = table.CreateSet<Person>();
            people.First().FirstName.Should().Be("John");
        }

        [Test]
        public void Sets_properties_from_column_names_with_underscore_to_properties_with_underscore()
        {
            var table = new Table("With_Underscore");
            table.AddRow("John");
            var people = table.CreateSet<Person>();
            people.First().With_Underscore.Should().Be("John");
        }

        [Test]
        public void Sets_properties_from_column_names_to_properties_with_umlaute()
        {
            var table = new Table("WithUmlauteäöü");
            table.AddRow("John");
            var people = table.CreateSet<Person>();
            people.First().WithUmlauteäöü.Should().Be("John");
        }


        [Test]
        public void Sets_properties_from_column_names_to_properties_with_dash()
        {
            var table = new Table("first-name");
            table.AddRow("John");
            var people = table.CreateSet<Person>();
            people.First().FirstName.Should().Be("John");
        }

        [Test]
        public void Returns_two_instances_when_there_are_two_rows()
        {
            var table = new Table("FirstName");
            table.AddRow("John");
            table.AddRow("Howard");
            var people = table.CreateSet<Person>();
            people.Count().Should().Be(2);
        }

        [Test]
        public void Sets_string_values_on_the_instance_when_type_is_string()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("John", "Galt", "", "", "", "");

            var people = table.CreateSet<Person>();

            people.First().FirstName.Should().Be("John");
            people.First().LastName.Should().Be("Galt");
        }

        [Test]
        public void Sets_int_values_on_the_instance_when_type_is_int()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "3", "", "");

            var people = table.CreateSet<Person>();

            people.First().NumberOfIdeas.Should().Be(3);
        }



        [Test]
        public void Sets_Enum_values_on_the_instance_when_type_is_int()
        {
            var table = new Table("FirstName", "LastName", "BirthDate", "NumberOfIdeas", "Salary", "IsRational", "Sex");
            table.AddRow("", "", "", "", "", "", "Male");

            var people = table.CreateSet<Person>();

            people.First().Sex.Should().Be(Sex.Male);
        }

        [Test]
        public void Sets_datetime_on_the_instance_when_type_is_datetime()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "4/28/2009", "3", "", "");

            var people = table.CreateSet<Person>();

            people.First().BirthDate.Should().Be(new DateTime(2009, 4, 28));
        }

        
        [Test]
        public void Sets_decimal_on_the_instance_when_type_is_decimal()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "4/28/2009", "3", 9997.43M.ToString(), "");

            var people = table.CreateSet<Person>();

            people.First().Salary.Should().Be(9997.43M);
        }

        [Test]
        public void Sets_bools_on_the_instance_when_type_is_bool()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "4/28/2009", "3", "", "true");

            var people = table.CreateSet<Person>();

            people.First().IsRational.Should().BeTrue();
        }

        [Test]
        public void Sets_doubles_on_the_instance_when_type_is_double()
        {
            var table = new Table("Double", "NullableDouble");
            table.AddRow("4.193", "7.28");

            var people = table.CreateSet<Person>();

            people.First().Double.Should().Be(4.193);
            people.First().NullableDouble.Should().Be(7.28);
        }

        [Test]
        public void Sets_floats_on_the_instance_when_type_is_float()
        {
            var table = new Table("Float", "NullableFloat");
            table.AddRow("2.698", "8.954");

            var people = table.CreateSet<Person>();

            people.First().Float.Should().Be(2.698F);
            people.First().NullableFloat.Should().Be(8.954F);
        }

        [Test]
        public void Sets_guids_on_the_instance_when_the_type_is_guid()
        {
            var table = new Table("GuidId", "NullableGuidId");
            table.AddRow("8A6F6A2F-4EF8-4D6A-BCCE-749E8513BA82", "11116FB0-3E49-473A-B79F-A77D0A5A1526");

            var people = table.CreateSet<Person>();

            people.First().GuidId.Should().Be(new Guid("8A6F6A2F-4EF8-4D6A-BCCE-749E8513BA82"));
            people.First().NullableGuidId.Should().Be(new Guid("11116FB0-3E49-473A-B79F-A77D0A5A1526"));
        }

        [Test]
        public void Sets_uints_on_the_instance_when_the_type_is_uint()
        {
            var table = new Table("UnsignedInt", "NullableUnsignedInt");
            table.AddRow("1", "2");

            var people = table.CreateSet<Person>();

            people.First().UnsignedInt.Should().Be(1);
            people.First().NullableUnsignedInt.Should().Be((uint?)2);
        }

        [Test]
        public void Sets_chars_on_the_instance_when_the_type_is_char()
        {
            var table = new Table("MiddleInitial", "NullableChar");
            table.AddRow("O", "K");

            var people = table.CreateSet<Person>();

            people.First().MiddleInitial.Should().Be('O');
            people.First().NullableChar.Should().Be('K');
        }

        [Test]
        public void Works_with_valueTuples()
        {
            var table = new Table("Name", "Age", "HiScore", "ShoeSize");
            table.AddRow("Rich", "48", "345467", "9.5");
            table.AddRow("Sarah", "45", "12654", "4.0");

            var people = table.CreateSet<(string Name, int Age, int HiScore, decimal ShoeSize)>();

            people.First().Name.Should().Be("Rich");
            people.First().Age.Should().Be(48);
            people.First().HiScore.Should().Be(345467);
            people.First().ShoeSize.Should().Be(9.5m);
            people.Last().Name.Should().Be("Sarah");
            people.Last().Age.Should().Be(45);
            people.Last().HiScore.Should().Be(12654);
            people.Last().ShoeSize.Should().Be(4.0m);
        }
    }
}