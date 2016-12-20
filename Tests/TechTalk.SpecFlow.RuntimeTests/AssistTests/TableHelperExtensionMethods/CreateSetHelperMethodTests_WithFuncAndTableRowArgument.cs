using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    [TestFixture]
    public class CreateSetHelperMethodTests_WithFuncAndTableRowArgument
    {
        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        [Test]
        public void Uses_the_instance_creation_method_when_passed_one_row()
        {
            var table = new Table("FirstName");
            table.AddRow("John");

            var expectedPerson = new Person();

            var people = table.CreateSet(row => expectedPerson);

            people.Single().Should().Be(expectedPerson);
        }

        [Test]
        public void Calls_the_instance_creation_method_for_each_row()
        {
            var table = new Table("FirstName");
            table.AddRow("one");
            table.AddRow("two");

            var first = new Person();
            var second = new Person();

            var queue = new Queue<Person>();
            queue.Enqueue(first);
            queue.Enqueue(second);

            var people = table.CreateSet(row => queue.Dequeue());

            people.Count().Should().Be(2);
            people.First().Should().Be(first);
            people.Last().Should().Be(second);
        }

        [Test]
        public void Still_loads_the_instance_with_the_values_from_the_table()
        {
            var table = new Table("FirstName");
            table.AddRow("John");
            table.AddRow("Howard");

            var john = new Person();
            var howard = new Person();

            var queue = new Queue<Person>();
            queue.Enqueue(john);
            queue.Enqueue(howard);

            var people = table.CreateSet(row => queue.Dequeue());

            people.First().FirstName.Should().Be("John");
            people.Last().FirstName.Should().Be("Howard");
        }

        [Test]
        public void The_correct_TableRow_is_passed_to_the_callback()
        {
            var table = new Table("FirstName", "_SpecialInfo");
            table.AddRow("John", "John Info");
            table.AddRow("Howard", "Howard Info");

            var people = table.CreateSet(row => new Person() {LastName = row["_SpecialInfo"]});

            foreach (var person in people)
                LastNameString_ShouldStartWith_FirstNameString(person);
        }

        [Test]
        public void It_should_be_easy_to_interact_with_the_row()
        {
            var table = new Table("First Name", "Birth Date");
            table.AddRow("John", "1/1/1900");
            table.AddRow("Howard", "2/2/1920");

            var people = table.CreateSet(row => new Person
            {
                FirstName = row["First Name"],
                BirthDate = new DateTimeValueRetriever().GetValue(row["Birth Date"]),
            });

            people.First().FirstName.Should().Equals("John");
            people.Last().FirstName.Should().Equals("Howard");

            people.First().BirthDate.Should().Equals(DateTime.Parse("1/1/1900"));
            people.Last().BirthDate.Should().Equals(DateTime.Parse("2/2/1920"));
        }

        private static void LastNameString_ShouldStartWith_FirstNameString(Person person)
        {
            person.LastName.Should().StartWith(person.FirstName);
        }
    }
}