using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.Assist;

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
    }
}
