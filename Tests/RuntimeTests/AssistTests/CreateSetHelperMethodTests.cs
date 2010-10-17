using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;
namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    [TestFixture]
    public class CreateSetHelperMethodTests
    {
        [SetUp]
        public void TestSetup()
        {
            // this is required, because the tests depend on parsing decimals with the en-US culture
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
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
            var table = new Table("FirstName", "LastName", "BirthDate", "NumberOfIdeas", "Salary", "IsRational");
            table.AddRow("John", "Galt", "", "", "", "");

            var people = table.CreateSet<Person>();

            people.First().FirstName.ShouldEqual("John");
            people.First().LastName.ShouldEqual("Galt");
        }

        [Test]
        public void Sets_int_values_on_the_instance_when_type_is_int()
        {
            var table = new Table("FirstName", "LastName", "BirthDate", "NumberOfIdeas", "Salary", "IsRational");
            table.AddRow("", "", "", "3", "", "");

            var people = table.CreateSet<Person>();

            people.First().NumberOfIdeas.ShouldEqual(3);
        }

        [Test]
        public void Sets_datetime_on_the_instance_when_type_is_datetime()
        {
            var table = new Table("FirstName", "LastName", "BirthDate", "NumberOfIdeas", "Salary", "IsRational");
            table.AddRow("", "", "4/28/2009", "3", "", "");

            var people = table.CreateSet<Person>();

            people.First().BirthDate.ShouldEqual(new DateTime(2009, 4, 28));
        }

        [Test]
        public void Sets_decimal_on_the_instance_when_type_is_decimal()
        {
            var table = new Table("FirstName", "LastName", "BirthDate", "NumberOfIdeas", "Salary", "IsRational");
            table.AddRow("", "", "4/28/2009", "3", "9997.43", "");

            var people = table.CreateSet<Person>();

            people.First().Salary.ShouldEqual(9997.43M);
        }

        [Test]
        public void Sets_booleans_on_the_instance_when_type_is_boolean()
        {
            var table = new Table("FirstName", "LastName", "BirthDate", "NumberOfIdeas", "Salary", "IsRational");
            table.AddRow("", "", "4/28/2009", "3", "", "true");

            var people = table.CreateSet<Person>();

            people.First().IsRational.ShouldBeTrue();
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int NumberOfIdeas { get; set; }
        public decimal Salary { get; set; }
        public bool IsRational { get; set; }

        public DateTime? NullableDateTime { get; set; }
        public bool? NullableBool { get; set; }
        public decimal? NullableDecimal { get; set; }
        public int? NullableInt { get; set; }
    }
}
