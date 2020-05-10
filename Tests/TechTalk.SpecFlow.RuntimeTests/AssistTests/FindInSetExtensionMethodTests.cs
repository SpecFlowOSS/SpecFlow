using System;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    
    public class FindInSetExtensionMethodsTests
    {
        private IList<InstanceComparisonTestObject> testSet;

        
        public FindInSetExtensionMethodsTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            testSet = new List<InstanceComparisonTestObject>();
            testSet.Add(new InstanceComparisonTestObject { StringProperty = "Howard Rorak" });
            testSet.Add(new InstanceComparisonTestObject { StringProperty = "Joel Mario", IntProperty = 20 });
        }

        [Fact]
        public void It_returns_null_when_no_match_can_be_found()
        {
            var table = new Table("Field", "Value");
            table.AddRow("String Property", "Peter Keating");

            table.FindInSet(testSet).Should().BeNull();
        }

        [Fact]
        public void Returns_instance_on_full_match()
        {
            var table = new Table("Field", "Value");
            table.AddRow("String Property", "Joel Mario");
            table.AddRow("Int Property", "20");

            var result = table.FindInSet(testSet);

            result.Should().BeSameAs(testSet[1]);
        }

        [Fact]
        public void Returns_instance_on_partial_match()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Int Property", "20");

            var result = table.FindInSet(testSet);

            result.Should().BeSameAs(testSet[1]);
        }

        [Fact]
        public void Returns_null_if_the_property_cannot_be_found()
        {
            var table = new Table("Field", "Value");
            table.AddRow("What You Talkin Bout", "Willis");

            table.FindInSet(testSet).Should().BeNull();
        }

        [Fact]
        public void It_should_throw_if_multiple_results_can_be_found()
        {
            var john = new Person {FirstName = "John", LastName = "Doe"};
            var jane = new Person {FirstName = "Jane", LastName = "Doe"};
            var records = new List<Person> { john, jane};

            var table = new Table("Field", "Value");
            table.AddRow("LastName", "Doe");

            Exception exception = null;
            try
            {
                table.FindInSet(records);
            }
            catch (ComparisonException ex)
            {
                exception = ex;
            }

            exception.Should().NotBeNull();
            exception.Message.Should().Be("Multiple instances match the table");
        }

        [Fact]
        public void Usage_example()
        {
            var howardRoark = new Person {FirstName = "Howard", LastName = "Roark"};
            var johnGalt = new Person {FirstName = "John", LastName = "Galt"};
            var records = new List<Person> {howardRoark, johnGalt};

            Table table;
            table = new Table("Field", "Value");
            table.AddRow("FirstName", "Howard");

            table.FindInSet(records).Should().Be(howardRoark);

            table = new Table("Field", "Value");
            table.AddRow("LastName", "Galt");

            table.FindInSet(records).Should().Be(johnGalt);

            table = new Table("Field", "Value");
            table.AddRow("LastName", "Keating");

            table.FindInSet(records).Should().BeNull();
        }

        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        private ComparisonTestResult ExceptionWasThrownByThisSearch(Table table, IEnumerable<InstanceComparisonTestObject> set)
        {
            var result = new ComparisonTestResult { ExceptionWasThrown = false };
            try
            {
                table.FindInSet(set);
            }
            catch (ComparisonException ex)
            {
                result.ExceptionWasThrown = true;
                result.ExceptionMessage = ex.Message;
            }
            return result;
        }
    }
}