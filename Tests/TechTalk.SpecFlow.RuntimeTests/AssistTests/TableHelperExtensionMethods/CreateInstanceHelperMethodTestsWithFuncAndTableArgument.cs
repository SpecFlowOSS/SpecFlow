using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
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

    }
}