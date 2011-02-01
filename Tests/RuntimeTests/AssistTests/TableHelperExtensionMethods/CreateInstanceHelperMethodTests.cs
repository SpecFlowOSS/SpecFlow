using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    [TestFixture]
    public class CreateInstanceHelperMethodTests
    {
        [SetUp]
        public void SetUp()
        {
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
        public void Sets_properties_with_different_case()
        {
            var table = new Table("Field", "Value");
            table.AddRow("firstname", "John");

            var person = table.CreateInstance<Person>();

            person.FirstName.ShouldEqual("John");
        }

        [Test]
        public void Sets_properties_from_column_names_with_blanks()
        {
            var table = new Table("Field", "Value");
            table.AddRow("First Name", "John");

            var person = table.CreateInstance<Person>();

            person.FirstName.ShouldEqual("John");
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
        public void Sets_enum_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Sex", "Male");

            var person = table.CreateInstance<Person>();

            person.Sex.ShouldEqual(Sex.Male);
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
        public void Sets_decimal_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Salary", 9.78M.ToString());

            var person = table.CreateInstance<Person>();

            person.Salary.ShouldEqual(9.78M);
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
        public void Sets_datetime_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("BirthDate", "12/31/2010");
            table.AddRow("NullableDateTime", "11/30/2011");

            var person = table.CreateInstance<Person>();

            person.BirthDate.ShouldEqual(new DateTime(2010, 12, 31));
            person.NullableDateTime.ShouldEqual(new DateTime(2011, 11, 30));
        }

        [Test]
        public void Sets_char_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("MiddleInitial", "T");
            table.AddRow("NullableChar", "S");

            var person = table.CreateInstance<Person>();

            person.MiddleInitial.ShouldEqual('T');
            person.NullableChar.ShouldEqual('S');
        }
    }
}