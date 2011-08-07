using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Should;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    [TestFixture]
    public abstract class CreateInstanceHelperMethodTestBase
    {
        private readonly Func<Table, Person> func;

        public CreateInstanceHelperMethodTestBase(Func<Table, Person> func)
        {
            this.func = func;
        }

        public Person GetThePerson(Table table)
        {
            return func(table);
        }

        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        [Test]
        public virtual void Sets_properties_with_different_case()
        {
            var table = new Table("Field", "Value");
            table.AddRow("firstname", "John");

            var person = GetThePerson(table);

            person.FirstName.ShouldEqual("John");
        }

        [Test]
        public virtual void Sets_properties_from_column_names_with_blanks()
        {
            var table = new Table("Field", "Value");
            table.AddRow("First Name", "John");

            var person = GetThePerson(table);

            person.FirstName.ShouldEqual("John");
        }

        [Test]
        public virtual void Sets_string_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("FirstName", "John");
            table.AddRow("LastName", "Galt");

            var person = GetThePerson(table);

            person.FirstName.ShouldEqual("John");
            person.LastName.ShouldEqual("Galt");
        }

        [Test]
        public virtual void Sets_enum_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Sex", "Male");

            var person = GetThePerson(table);

            person.Sex.ShouldEqual(Sex.Male);
        }

        [Test]
        public virtual void Sets_int_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("NumberOfIdeas", "3");

            var person = GetThePerson(table);

            person.NumberOfIdeas.ShouldEqual(3);
        }

        [Test]
        public virtual void Sets_uint_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("UnsignedInt", "3");

            var person = GetThePerson(table);

            person.UnsignedInt.ShouldEqual<uint>(3);
        }

        [Test]
        public virtual void Sets_decimal_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Salary", 9.78M.ToString());

            var person = GetThePerson(table);

            person.Salary.ShouldEqual(9.78M);
        }

        [Test]
        public virtual void Sets_bool_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("IsRational", "true");

            var person = GetThePerson(table);

            person.IsRational.ShouldBeTrue();
        }

        [Test]
        public virtual void Sets_datetime_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("BirthDate", "12/31/2010");
            table.AddRow("NullableDateTime", "11/30/2011");

            var person = GetThePerson(table);

            person.BirthDate.ShouldEqual(new DateTime(2010, 12, 31));
            person.NullableDateTime.ShouldEqual(new DateTime(2011, 11, 30));
        }

        [Test]
        public virtual void Sets_char_values()
        {
            var table = new Table("Field", "Value");
            table.AddRow("MiddleInitial", "T");
            table.AddRow("NullableChar", "S");

            var person = GetThePerson(table);

            person.MiddleInitial.ShouldEqual('T');
            person.NullableChar.ShouldEqual('S');
        }
    }
}