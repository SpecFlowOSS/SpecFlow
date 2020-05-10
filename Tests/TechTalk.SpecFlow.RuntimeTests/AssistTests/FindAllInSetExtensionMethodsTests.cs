using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.RuntimeTests.AssistTests.TestInfrastructure;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{
    
    public class FindAllInSetExtensionMethodsTests
    {
        private IList<InstanceComparisonTestObject> testSet;
        public FindAllInSetExtensionMethodsTests()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            testSet = new List<InstanceComparisonTestObject>();
            testSet.Add(new InstanceComparisonTestObject { StringProperty = "Howard Rorak" });
            testSet.Add(new InstanceComparisonTestObject { StringProperty = "Joel Mario", IntProperty = 20 });
        }

        [Fact]
        public void It_returns_nothing_when_no_match_could_be_found()
        {
            var table = new Table("Field", "Value");
            table.AddRow("String Property", "Peter Keating");

            table.FindAllInSet(testSet).Count().Should().Be(0);
        }

        [Fact]
        public void Returns_match_on_full_match()
        {
            var table = new Table("Field", "Value");
            table.AddRow("String Property", "Joel Mario");
            table.AddRow("Int Property", "20");

            var results = table.FindAllInSet(testSet);

            results.Count().Should().Be(1);
            results.First().IntProperty.Should().Be(20);
            results.First().StringProperty.Should().Be("Joel Mario");
        }

        [Fact]
        public void Returns_match_on_partial_match()
        {
            var table = new Table("Field", "Value");
            table.AddRow("Int Property", "20");

            var results = table.FindAllInSet(testSet);

            results.Count().Should().Be(1);
            results.First().IntProperty.Should().Be(20);
        }

        [Fact]
        public void Returns_nothing_if_the_match_cannot_be_found()
        {
            var table = new Table("Field", "Value");
            table.AddRow("What You Talkin Bout", "Willis");

            table.FindAllInSet(testSet).Count().Should().Be(0);
        }

        [Fact]
        public void It_should_return_multiple_matches_if_they_can_be_Found()
        {
            var john = new Person {FirstName = "John", LastName = "Doe"};
            var jane = new Person {FirstName = "Jane", LastName = "Doe"};
            var records = new List<Person> { john, jane};

            var table = new Table("Field", "Value");
            table.AddRow("LastName", "Doe");

            var results = table.FindAllInSet(records);
            results.Count().Should().Be(2);
            results.Should().Contain(john);
            results.Should().Contain(jane);
        }

        [Fact]
        public void Usage_example()
        {
            var john = new Person {FirstName = "John", LastName = "Doe"};
            var jane = new Person {FirstName = "Jane", LastName = "Doe"};
            var records = new List<Person> {john, jane};

            Table table;
            table = new Table("Field", "Value");
            table.AddRow("FirstName", "John");

            table.FindAllInSet(records).Count().Should().Be(1);
            table.FindAllInSet(records).Should().Contain(john);

            table = new Table("Field", "Value");
            table.AddRow("LastName", "Doe");

            table.FindAllInSet(records).Count().Should().Be(2);
            table.FindAllInSet(records).Should().Contain(john);
            table.FindAllInSet(records).Should().Contain(jane);

            table = new Table("Field", "Value");
            table.AddRow("LastName", "Trump");

            table.FindAllInSet(records).Count().Should().Be(0);
        }

        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}