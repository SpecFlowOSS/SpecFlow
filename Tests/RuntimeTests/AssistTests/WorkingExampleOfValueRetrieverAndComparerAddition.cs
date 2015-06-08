using System;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using System.Collections.Generic;
using FluentAssertions;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{

    public class FancyName
    {
        public string FirstName { get; set; }
        public string LastName;
    }

    public class FancyLad
    {
        public FancyName Name { get; set; }
    }

    public class FancyNameValueRetriever : IValueRetriever
    {

        public static FancyName Parse(string fullName)
        {
            var firstName = fullName.ToString().Split(' ').First();
            var lastName  = fullName.ToString().Split(' ').Last();
            return new FancyName() { FirstName = firstName, LastName = lastName };
        }

        public IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(FancyName) };
        }

        public object ExtractValueFromRow(TableRow row, Type targetType)
        {
            return FancyNameValueRetriever.Parse(row[1]);
        }
    }

    public class FancyNameValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof(FancyName);
        }

        public bool TheseValuesAreTheSame(string expectedValue, object actualValue)
        {
            var expected = FancyNameValueRetriever.Parse(expectedValue);
            var actual = (FancyName)actualValue; 
            return expected.FirstName == actual.FirstName && expected.LastName == actual.LastName;
        }
    }

    [TestFixture]
    public class WorkingExampleOfValueRetrieverAndComparerAddition
    {
        [TearDown]
        public void Cleanup()
        {
            Service.Instance.RestoreDefaults();
        }

        [Test]
        public void Should_be_able_to_retrieve_the_fancy_name()
        {

            Service.Instance.RegisterValueRetriever(new FancyNameValueRetriever());

            var table = new Table("Field", "Value");
            table.AddRow("Name", "John Galt");

            var lad = table.CreateInstance<FancyLad>();

            lad.Name.FirstName.Should().Be("John");
            lad.Name.LastName.Should().Be("Galt");
        }

        [Test]
        public void Should_be_able_to_compare_the_fancy_name()
        {
            Service.Instance.RegisterValueRetriever(new FancyNameValueRetriever());
            Service.Instance.RegisterValueComparer(new FancyNameValueComparer());

            var table = new Table("Field", "Value");
            table.AddRow("Name", "John ignore these values in the middle Galt");

            var expectedName = new FancyName() { FirstName = "John", LastName = "Galt" };
            var expectedLad  = new FancyLad() { Name = expectedName };

            table.CompareToInstance<FancyLad>(expectedLad);
        }
    }
}
