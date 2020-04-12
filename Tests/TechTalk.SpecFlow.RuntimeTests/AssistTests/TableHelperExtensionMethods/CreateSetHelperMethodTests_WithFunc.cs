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
    
    public class CreateSetHelperMethodTests_WithFunc
    {
        public CreateSetHelperMethodTests_WithFunc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
        }

        [Fact]
        public void Uses_the_instance_creation_method_when_passed_one_row()
        {
            var table = new Table("FirstName");
            table.AddRow("John");

            var expectedPerson = new Person();
            
            var people = table.CreateSet(() => expectedPerson);

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

            var people = table.CreateSet(queue.Dequeue);

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

            var people = table.CreateSet(queue.Dequeue);

            people.First().FirstName.Should().Be("John");
            people.Last().FirstName.Should().Be("Howard");
        }
    }
}