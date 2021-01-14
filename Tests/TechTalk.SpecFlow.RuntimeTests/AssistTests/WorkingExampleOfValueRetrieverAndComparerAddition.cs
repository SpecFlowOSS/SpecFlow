using System;
using System.Linq;
using Xunit;
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

        public override string ToString()
        {
            return String.Join(" ", new string[]{ FirstName, LastName });
        }
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

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return FancyNameValueRetriever.Parse(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type type)
        {
            return TypesForWhichIRetrieveValues().Contains(type);
        }

    }

    public class FancyNameValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof(FancyName);
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            var expected = FancyNameValueRetriever.Parse(expectedValue);
            var actual = (FancyName)actualValue; 
            return expected.FirstName == actual.FirstName && expected.LastName == actual.LastName;
        }
    }

    
    public class WorkingExampleOfValueRetrieverAndComparerAddition : IDisposable
    {

        [Fact]
        public void Should_be_able_to_retrieve_the_fancy_name()
        {

            Service.Instance.ValueRetrievers.Register(new FancyNameValueRetriever());

            var table = new Table("Field", "Value");
            table.AddRow("Name", "John Galt");

            var lad = table.CreateInstance<FancyLad>();

            lad.Name.FirstName.Should().Be("John");
            lad.Name.LastName.Should().Be("Galt");
        }

        [Fact]
        public void Should_be_able_to_compare_the_fancy_name()
        {
            Service.Instance.ValueRetrievers.Register(new FancyNameValueRetriever());
            Service.Instance.ValueComparers.Register(new FancyNameValueComparer());

            var table = new Table("Field", "Value");
            table.AddRow("Name", "John Galt");

            var expectedName = new FancyName() { FirstName = "John", LastName = "Galt" };
            var expectedLad  = new FancyLad() { Name = expectedName };

            table.CompareToInstance<FancyLad>(expectedLad);
        }

        public void Dispose()
        {
            Service.Instance.RestoreDefaults();
        }
    }


    /***************************************************/

    public class Product
    {
        public string Name { get; set; }
        public ProductCategory Category { get; set; }
    }

    public class ProductCategory
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name + "!!!";
        }
    }

    public class ProductCategoryValueRetriever : IValueRetriever
    {

        public static ProductCategory Parse(string name)
        {
            return new ProductCategory() { Name = name };
        }

        public IEnumerable<Type> TypesForWhichIRetrieveValues()
        {
            return new Type[]{ typeof(ProductCategory) };
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return ProductCategoryValueRetriever.Parse(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type type)
        {
            return TypesForWhichIRetrieveValues().Contains(type);
        }

    }

    public class ProductCategoryValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue != null && actualValue.GetType() == typeof(ProductCategory);
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            var expected = ProductCategoryValueRetriever.Parse(expectedValue);
            var actual = (ProductCategory)actualValue; 
            return expected.Name == actual.Name;
        }
    }

    
    public class AnotherWorkingExampleOfValueRetrieverAndComparerAddition : IDisposable
    {
        [Fact]
        public void Should_be_able_to_retrieve_the_category()
        {

            Service.Instance.ValueRetrievers.Register(new ProductCategoryValueRetriever());

            var table = new Table("Field", "Value");
            table.AddRow("Name", "Apple");
            table.AddRow("Category", "Fruit");

            var lad = table.CreateInstance<Product>();

            lad.Name.Should().Be("Apple");
            lad.Category.Name.Should().Be("Fruit");
        }

        [Fact]
        public void Should_be_able_to_compare_the_category()
        {
            Service.Instance.ValueRetrievers.Register(new ProductCategoryValueRetriever());
            Service.Instance.ValueComparers.Register(new ProductCategoryValueComparer());

            var table = new Table("Field", "Value");
            table.AddRow("Name", "Cucumber");
            table.AddRow("Category", "Vegetable");

            var expectedCategory = new ProductCategory() { Name = "Vegetable" };
            var expectedProduct = new Product() { Name = "Cucumber", Category = expectedCategory };

            table.CompareToInstance<Product>(expectedProduct);
        }

        public void Dispose()
        {
            Service.Instance.RestoreDefaults();

        }
    }
}
