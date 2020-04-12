using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    
    public class CreateSetHelperMethodTests_WithFuncAndTableRowArgument
    {
        public CreateSetHelperMethodTests_WithFuncAndTableRowArgument()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Fact]
        public void Uses_the_instance_creation_method_when_passed_one_row()
        {
            var table = new Table("FirstName");
            table.AddRow("John");

            var expectedPerson = new Person();

            var people = table.CreateSet(row => expectedPerson);

            people.Single().Should().Be(expectedPerson);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void The_correct_TableRow_is_passed_to_the_callback()
        {
            var table = new Table("FirstName", "_SpecialInfo");
            table.AddRow("John", "John Info");
            table.AddRow("Howard", "Howard Info");

            var people = table.CreateSet(row => new Person() {LastName = row["_SpecialInfo"]});

            foreach (var person in people)
                LastNameString_ShouldStartWith_FirstNameString(person);
        }

        private static void LastNameString_ShouldStartWith_FirstNameString(Person person)
        {
            person.LastName.Should().StartWith(person.FirstName);
        }
    }
}