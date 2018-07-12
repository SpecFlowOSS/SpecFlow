using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    [TestFixture]
    public class CreateInstanceHelperMethodTestsWithFuncAndTableArgument
    {
        [Test]
        public void CreateInstance_returns_the_object_returned_from_the_func()
        {
            var table = new Table("Field", "Value");
            var expectedPerson = new Person();
            var person = table.CreateInstance(tbl => expectedPerson);
            person.Should().Be(expectedPerson);
        }

        [Test]
        public void Create_instance_will_fill_the_instance_()
        {
            var table = new Table("Field", "Value");
            table.AddRow("FirstName", "John");
            table.AddRow("LastName", "Galt");

            var expectedPerson = new Person { FirstName = "Ellsworth", LastName = "Toohey" };
            var person = table.CreateInstance(tbl => expectedPerson);

            person.FirstName.Should().Be("John");
            person.LastName.Should().Be("Galt");
        }

        [Test]
        public void Create_instance_use_the_correct_table_in_callback()
        {
            var table = new Table("Field", "Value");

            Table tableFromCallback = null;
            var person = table.CreateInstance(tbl =>
            {
                tableFromCallback = tbl;
                return new Person();
            });

            tableFromCallback.Should().Be(table);
        }

        [Test]
        public void CreateInstance_works_with_a_horizontal_table()
        {
            var table = new Table("First Name", "Last Name");
            table.AddRow("John", "Galt");

            var expectedPerson = new Person { FirstName = "John", LastName = "Galt" };
            var person = table.CreateInstance(tbl => expectedPerson);

            person.FirstName.Should().Be("John");
            person.LastName.Should().Be("Galt");
        }

        [Test]
        public void CreateInstance_should_be_easy_to_work_with()
        {
            var table = new Table("First Name", "Last Name", "Birth Date");
            table.AddRow("John", "Galt", "1/1/1940");

            var person = table.CreateInstance(tbl =>
            {
                // this is going to fail, because the table
                // here has been flipped to the Field/Value form
                // even though it is in a horizontal form above
                return new Person
                {
                    FirstName = tbl.Rows[0]["FirstName"],
                    LastName = tbl.Rows[0]["LastName"],
                    BirthDate = new DateTimeValueRetriever().GetValue(tbl.Rows[0]["BirthDate"]),
                };
            });

            person.FirstName.Should().Be("John");
            person.LastName.Should().Be("Galt");
            person.BirthDate.Should().Be(DateTime.Parse("1/1/1940"));
        }
    }
}