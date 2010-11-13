using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Should;
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
            return new Table("FirstName", "LastName", "BirthDate", "NumberOfIdeas", "Salary", "IsRational", "Sex");
        }

        [Test]
        public void Returns_empty_set_of_type_when_there_are_no_rows()
        {
            var table = new Table("FirstName");
            var people = table.CreateSet<Person>();
            people.Count().ShouldEqual(0);
        }

        [Test]
        public void Returns_one_instance_when_there_is_one_row()
        {
            var table = new Table("FirstName");
            table.AddRow("John");
            var people = table.CreateSet<Person>();
            people.Count().ShouldEqual(1);
        }

        [Test]
        public void Returns_two_instances_when_there_are_two_rows()
        {
            var table = new Table("FirstName");
            table.AddRow("John");
            table.AddRow("Howard");
            var people = table.CreateSet<Person>();
            people.Count().ShouldEqual(2);
        }

        [Test]
        public void Sets_string_values_on_the_instance_when_type_is_string()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("John", "Galt", "", "", "", "", "");

            var people = table.CreateSet<Person>();

            people.First().FirstName.ShouldEqual("John");
            people.First().LastName.ShouldEqual("Galt");
        }

        [Test]
        public void Sets_int_values_on_the_instance_when_type_is_int()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "3", "", "", "");

            var people = table.CreateSet<Person>();

            people.First().NumberOfIdeas.ShouldEqual(3);
        }



        [Test]
        public void Sets_Enum_values_on_the_instance_when_type_is_int()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "", "", "", "", "Male");

            var people = table.CreateSet<Person>();

            people.First().Sex.ShouldEqual(Sex.Male);
        }

        [Test]
        public void Sets_datetime_on_the_instance_when_type_is_datetime()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "4/28/2009", "3", "", "", "");

            var people = table.CreateSet<Person>();

            people.First().BirthDate.ShouldEqual(new DateTime(2009, 4, 28));
        }

        [Test]
        public void Sets_decimal_on_the_instance_when_type_is_decimal()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "4/28/2009", "3", 9997.43M.ToString(), "", "");

            var people = table.CreateSet<Person>();

            people.First().Salary.ShouldEqual(9997.43M);
        }

        [Test]
        public void Sets_booleans_on_the_instance_when_type_is_boolean()
        {
            var table = CreatePersonTableHeaders();
            table.AddRow("", "", "4/28/2009", "3", "", "true", "");

            var people = table.CreateSet<Person>();

            people.First().IsRational.ShouldBeTrue();
        }
    }
}