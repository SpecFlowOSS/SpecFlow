using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests
{

    public class FancyName
    {
        public string FirstName { get; set; }
        public string LastName;

        public override string ToString()
        {
            return string.Join(" ", new string[]{ FirstName, LastName });
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
            return Parse(keyValuePair.Value);
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
            return Parse(keyValuePair.Value);
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

    /***************************************************/

    public class ParentItem
    {
        public string Name { get; set; }

        public ChildItem DutchItem { get; set; }

        public ChildItem EnglishItem { get; set; }
    }

    public class ChildItem
    {
        public string ItemName { get; set; }

        public string Language { get; set; }
    }

    /// <summary>
    /// Retrieves a ChildItem-objects for column's "Dutch name" and "English name" and maps them
    /// to the corresponding "DutchItem" and "EnglishItem" properties of a ParentItem-object.
    /// </summary>
    public class ChildItemValueRetriever : IValueRetriever
    {
        private static readonly IEnumerable<Type> TypesForWhichIRetrieveValues = new Type[] { typeof(ChildItem) };

        private static readonly IDictionary<string, string> ColumnNamesForWhichIRetrieveValues =
            new Dictionary<string, string>
            {
                { "Dutch name", "nl-NL" },
                { "English name", "en-US" }
            };

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type type)
        {
            return TypesForWhichIRetrieveValues.Contains(type) 
                && ColumnNamesForWhichIRetrieveValues.ContainsKey(keyValuePair.Key);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return Parse(keyValuePair.Key, keyValuePair.Value);
        }

        public static ChildItem Parse(string columnName, string columnValue)
        {
            return new ChildItem
            {
                ItemName = columnValue,
                Language = ColumnNamesForWhichIRetrieveValues[columnName]
            };
        }
    }


    public class ChildItemValueRetrieverTests : IDisposable
    {
        [Fact]
        public void IValueRetrievers_should_be_able_to_retrieve_the_child_item_by_column_name_registered_with_instance()
        {
            // Arrange
            Service.Instance.ValueRetrievers.Register(new ChildItemValueRetriever());

            var table = new Table("Name", "Dutch name", "English name");
            table.AddRow("Yellow fruit", "Ananas", "Pineapple");
            table.AddRow("Red fruit", "Appel", "Apple");

            // Act
            var parentItems = table.CreateSet<ParentItem>();

            // Assert
            parentItems.Should().NotBeNull();
            parentItems.Should().HaveCount(table.RowCount);

            // Assert - item 1
            parentItems.ElementAt(0).Name.Should().Be("Yellow fruit");
            parentItems.ElementAt(0).DutchItem.Should().NotBeNull();
            parentItems.ElementAt(0).DutchItem.ItemName.Should().Be("Ananas");
            parentItems.ElementAt(0).DutchItem.Language.Should().Be("nl-NL");
            parentItems.ElementAt(0).EnglishItem.Should().NotBeNull();
            parentItems.ElementAt(0).EnglishItem.ItemName.Should().Be("Pineapple");
            parentItems.ElementAt(0).EnglishItem.Language.Should().Be("en-US");

            // Assert - item 2
            parentItems.ElementAt(1).Name.Should().Be("Red fruit");
            parentItems.ElementAt(1).DutchItem.Should().NotBeNull();
            parentItems.ElementAt(1).DutchItem.ItemName.Should().Be("Appel");
            parentItems.ElementAt(1).DutchItem.Language.Should().Be("nl-NL");
            parentItems.ElementAt(1).EnglishItem.Should().NotBeNull();
            parentItems.ElementAt(1).EnglishItem.ItemName.Should().Be("Apple");
            parentItems.ElementAt(1).EnglishItem.Language.Should().Be("en-US");
        }

        [Fact]
        public void IValueRetrievers_should_be_able_to_retrieve_the_child_item_by_column_name_registered_generic()
        {
            // Arrange
            Service.Instance.ValueRetrievers.Register<ChildItemValueRetriever>();

            var table = new Table("Name", "Dutch name", "English name");
            table.AddRow("Yellow fruit", "Ananas", "Pineapple");
            table.AddRow("Red fruit", "Appel", "Apple");

            // Act
            var parentItems = table.CreateSet<ParentItem>();

            // Assert
            parentItems.Should().NotBeNull();
            parentItems.Should().HaveCount(table.RowCount);

            // Assert - item 1
            parentItems.ElementAt(0).Name.Should().Be("Yellow fruit");
            parentItems.ElementAt(0).DutchItem.Should().NotBeNull();
            parentItems.ElementAt(0).DutchItem.ItemName.Should().Be("Ananas");
            parentItems.ElementAt(0).DutchItem.Language.Should().Be("nl-NL");
            parentItems.ElementAt(0).EnglishItem.Should().NotBeNull();
            parentItems.ElementAt(0).EnglishItem.ItemName.Should().Be("Pineapple");
            parentItems.ElementAt(0).EnglishItem.Language.Should().Be("en-US");

            // Assert - item 2
            parentItems.ElementAt(1).Name.Should().Be("Red fruit");
            parentItems.ElementAt(1).DutchItem.Should().NotBeNull();
            parentItems.ElementAt(1).DutchItem.ItemName.Should().Be("Appel");
            parentItems.ElementAt(1).DutchItem.Language.Should().Be("nl-NL");
            parentItems.ElementAt(1).EnglishItem.Should().NotBeNull();
            parentItems.ElementAt(1).EnglishItem.ItemName.Should().Be("Apple");
            parentItems.ElementAt(1).EnglishItem.Language.Should().Be("en-US");
        }

        public void Dispose()
        {
            Service.Instance.RestoreDefaults();

        }
    }
}
