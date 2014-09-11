using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities;
using ObjectAssertExtensions = Should.ObjectAssertExtensions;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.TableHelperExtensionMethods
{
    [TestFixture]
    public class CreateSetHelperMethodTests_WithFunc
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
            
            var people = table.CreateSet(() => expectedPerson);

            ObjectAssertExtensions.ShouldBeSameAs(people.Single(), expectedPerson);
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

            var people = table.CreateSet(queue.Dequeue);

            ObjectAssertExtensions.ShouldEqual(people.Count(), 2);
            ObjectAssertExtensions.ShouldBeSameAs(people.First(), first);
            ObjectAssertExtensions.ShouldBeSameAs(people.Last(), second);
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

            var people = table.CreateSet(queue.Dequeue);

            ObjectAssertExtensions.ShouldEqual(people.First().FirstName, "John");
            ObjectAssertExtensions.ShouldEqual(people.Last().FirstName, "Howard");
        }

        [Test]
        public void Calls_instance_creation_method_using_row_as_parameter()
        {
            var table = new Table("FullName", "Sex", "IsRational");
            table.AddRow("John Smith", "Male", "False");
            table.AddRow("Howard Jones", "Male", "True");
            
            var people = table.CreateSet(row => new Person { FirstName = row.GetString("FullName").Split(' ').First() });

            var john = people.First();
            var howard = people.Last();

            ObjectAssertExtensions.ShouldEqual(john.FirstName, "John");
            ObjectAssertExtensions.ShouldEqual(john.IsRational, false);
            ObjectAssertExtensions.ShouldEqual(howard.FirstName, "Howard");
            ObjectAssertExtensions.ShouldEqual(howard.IsRational, true);
        }
    }
}