# SpecFlow.Assist Helpers

To use these helpers, you need to add the `TechTalk.SpecFlow.Assist` namespace to the top of your file:

``` csharp
using TechTalk.SpecFlow.Assist;
```

When the helper methods expects a type, you can use:

- classes
- records (with C# 9)
- tuples

## CreateInstance<T>

`CreateInstance<T>` is an extension method of `Table` that will convert the table data to an object. For example, if you list data in a table that lists the values of your object like this:

```gherkin
Given I entered the following data into the new account form:
| Field              | Value      |
| Name               | John Galt  |
| Birthdate          | 2/2/1902   |
| HeightInInches     | 72         |
| BankAccountBalance | 1234.56    |
```

... or in a horizontal table like this:

```gherkin
Given I entered the following data into the new account form:
| Name      | Birthdate | HeightInInches | BankAccountBalance |
| John Galt | 2/2/1902  | 72             | 1234.56            |
```

... you can convert the data in the table to an instance of an object like this:

``` csharp
[Given(@"Given I entered the following data into the new account form:")]
public void x(Table table)
{
    var account = table.CreateInstance<Account>();
    // account.Name will equal "John Galt", HeightInInches will equal 72, etc.
}
```

The `CreateInstance<T>` method will create the Account object and set properties according to what can be read from the table. It also uses the appropriate casting or conversion to turn your string into the appropriate type.

The headers in this table can be anything you want, e.g. "Field" and "Value". What matters is that the first column contains the property name and the second column the value.

Alternatively you can use ValueTuples and destructuring:

``` csharp
[Given(@"Given I entered the following data into the new account form:")]
public void x(Table table)
{
    var account = table.CreateInstance<Account>();
    var account = table.CreateInstance<(string name, DateTime birthDate, int heightInInches, decimal bankAccountBalance)>();
    // account.name will equal "John Galt", heightInInches will equal 72, etc.
}
```

**Important:** In the case of tuples, _**you need to have the same number of parameters and types; parameter names do not matter**_, as ValueTuples do not hold parameter names at runtime using reflection.
**Scenarios with more than 7 properties are not currently supported, and you will receive an exception if you try to map more properties.** 

## CreateSet<T>

`CreateSet<T>` is an extension method of `Table` that converts table data into a set of objects. For example, assume you have the following step:

```gherkin
Given these products exist
| Sku              | Name             | Price |
| BOOK1            | Atlas Shrugged   | 25.04 |
| BOOK2            | The Fountainhead | 20.15 |
```

You can convert the data in the table to a set of objects like this:

``` csharp
[Given(@"Given these products exist")]
public void x(Table table)
{
    var products = table.CreateSet<Product>();
    // ...
}
```

The `CreateSet<T>` method returns an `IEnumerable<T>` based on the matching data in the table. It contains the values for each object, making appropriate type conversions from string to the related property.

## CompareToInstance<T>

`CompareToInstance<T>` makes it easy to compare the properties of an object against a table. For example, you have a class like this:

``` csharp
public class Person
{
    public string FirstName { get; set;}  
    public string LastName { get; set; }
    public int YearsOld { get; set; }
}
```

You want to compare it to a table in a step like this:

```gherkin
Then the person should have the following values
| Field     | Value |
| FirstName | John  |
| LastName  | Galt  |
| YearsOld  | 54    |
```
  
You can assert that the properties match with this simple step definition:

``` csharp
    [Binding]
    public class Binding
    {
        ScenarioContext _scenarioContext;

        public Binding(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Then("the person should have the following values")]
        public void x(Table table){
            // you don't have to get person this way, this is just for demo
            var person = _scenarioContext.Get<Person>();
        
            table.CompareToInstance<Person>(person);
        }
    }
```

If FirstName is not "John", LastName is not "Galt", or YearsOld is not 54, a descriptive error showing the differences is thrown.

If the values match, no exception is thrown, and SpecFlow continues to process your scenario.

## CompareToSet<T>

`CompareToSet<T>` makes it easy to compare the values in a table to a set of objects. For example, you have a class like this:

```csharp
public class Account
{
    public string Id { get; set;}
    public string FirstName { get; set;}
    public string LastName { get; set;}
    public string MiddleName { get; set;}
}
```

You want to test that your system returns a specific set of accounts, like this:

```gherkin
Then I get back the following accounts
| Id     | FirstName | LastName |
| 1      | John      | Galt     |
| 2      | Howard    | Roark    |
```

You can test you results with one call to CompareToSet<T>:

``` csharp
    [Binding]
    public class Binding
    {
        ScenarioContext _scenarioContext;

        public Binding(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Then("I get back the following accounts")]
        public void x(Table table){
            var accounts = _scenarioContext.Get<IEnumerable<Account>>();
        
            table.CompareToSet<Account>(accounts)
        }
    }
```

In this example, `CompareToSet<T>` checks that two accounts are returned, and only tests the properties you defined in the table. **It does not test the order of the objects, only that one was found that matches.**  If no record matching the properties in your table is found, an exception is thrown that includes the row number(s) that do not match up.

## Column naming

The SpecFlow Assist helpers use the values in your table to determine what properties to set in your object. However, the names of the columns do not need to match exactly - whitespace and casing is ignored. For example, the following two tables are treated as identical:

```gherkin
| FirstName | LastName | DateOfBirth | HappinessRating |
```

```gherkin
| First name | Last name | Date of birth | HAPPINESS rating |
```

This allows you to make your tables more readable to others.

## Aliasing

*(Note: Available with version 2.3 and later)*

If you have properties in your objects that are known by different terms within the business domain, these can be Aliased in your model by applying the attribute `TableAliases`. This attribute takes a collection of aliases as regular expressions that can be used to refer to the property in question.

For example, if you have an object representing an Employee, you might want to alias the `Surname` property:

``` csharp
    public class Employee
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [TableAliases("Last[]?Name", "Family[]?Name")]
        public string Surname { get; set; }
    }
```

Test writers can then refer to this property as Surname, Last Name, Lastname, Family Name or FamilyName, and it will still be mapped to the correct column.

The `TableAliases` attribute can be applied to a field, a property as a single attribute with multiple regular expressions, or as multiple attributes, depending on your preference.

## Extensions

Out-of-the-box, the SpecFlow table helpers knows how to handle most  csharp base types. Types like `String`, `Bool`, `Enum`, `Int`, `Decimal`, `DateTime`, etc. are all covered. The covered types can be found [here](https://github.com/techtalk/SpecFlow/tree/master/TechTalk.SpecFlow/Assist/ValueRetrievers). If you want to cover more types, including your own custom types, you can do so by registering your own instances of `IValueRetriever` and `IValueComparer`.

For example, you have a complex object like this:

``` csharp
    public class Shirt
    {
        public string Name { get; set; }
        public Color Color { get; set; }
    }
```

You have a table like this:

```gherkin
| Name | Color |
| XL   | Blue  |
| L    | Red   |
```

If you want to map `Blue` and `Red` to the appropriate instance of the `Color` class, you need to create an instance of `IValueRetriever` that can convert the strings to the `Color` instance.

You can register your custom `IValueRetriever` (and/or an instance of `IValueComparer` if you want to compare colors) like this:

``` csharp
[Binding]
public static class Hooks1
{
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Service.Instance.ValueRetrievers.Register(new ColorValueRetriever());
        Service.Instance.ValueComparers.Register(new ColorValueComparer());
    }
}
```

Examples on implementing these interfaces can be found as follows:

* [IValueRetriever](https://github.com/techtalk/SpecFlow/tree/v2/Runtime/Assist/ValueRetrievers)
* [IValueComparer](https://github.com/techtalk/SpecFlow/tree/v2/Runtime/Assist/ValueComparers)

### NullValueRetriever (from SpecFlow 3)

By default, non-specified (empty string) values are considered:

* An empty string for `String` and `System.Uri` values
* A null value for `Nullable<>` primitive types
* An error for non-nullable primitive types

To specify null values explicitly, add a `NullValueRetriever` to the set of registered retrievers, specifying the text to be treated as a null value, e.g.:

``` csharp
[Binding]
public static class Hooks1
{
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Service.Instance.ValueRetrievers.Register(new NullValueRetriever("<null>"));
    }
}
```

**Note:** The comparison is case-insensitive.

## Using ToProjection<T>, ToProjectionOfSet<T> and ToProjectionOfInstance<T> extension methods for LINQ-based instance and set comparison

**SpecFlow.Assist CompareToSet** Table extension method only checks for equivalence of collections which is a reasonable default. **SpecFlow.Assist** namespace also contains extension methods for With based operations.

Consider the following steps:

``` gherkin
    Scenario: Match
    When I have a collection
    | Artist | Album |
    | Beatles | Rubber Soul |
    | Pink Floyd | Animals |
    | Muse | Absolution |
    Then it should match
    | Artist | Album |
    | Beatles | Rubber Soul |
    | Pink Floyd | Animals |
    | Muse | Absolution |
    And it should match
    | Artist | Album |
    | Beatles | Rubber Soul |
    | Muse | Absolution |
    | Pink Floyd | Animals |
    And it should exactly match
    | Artist | Album |
    | Beatles | Rubber Soul |
    | Pink Floyd | Animals |
    | Muse | Absolution |
    But it should not match
    | Artist | Album |
    | Beatles | Rubber Soul |
    | Queen | Jazz |
    | Muse | Absolution |
    And it should not match
    | Artist | Album |
    | Beatles | Rubber Soul |
    | Muse | Absolution |
    And it should not exactly match
    | Artist | Album |
    | Beatles | Rubber Soul |
    | Muse | Absolution |
    | Pink Floyd | Animals |
```

With LINQ- based operations each of the above comparisons can be expressed using a single line of code

``` csharp
    [Binding]
    public class Binding
    {
        ScenarioContext _scenarioContext;

        public Binding(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [When(@"I have a collection")]
        public void WhenIHaveACollection(Table table)
        {
            var collection = table.CreateSet<Item>();
            _scenarioContext.Add("Collection", collection);
        }
    
        [Then(@"it should match")]
        public void ThenItShouldMatch(Table table)
        {
            var collection = _scenarioContext["Collection"] as IEnumerable<Item>;
            Assert.IsTrue(table.RowCount == collection.Count() && table.ToProjection<Item>().Except(collection.ToProjection()).Count() == 0);
        }
    
        [Then(@"it should exactly match")]
        public void ThenItShouldExactlyMatch(Table table)
        {
            var collection = _scenarioContext["Collection"] as IEnumerable<Item>;
            Assert.IsTrue(table.ToProjection<Item>().SequenceEqual(collection.ToProjection()));
        }
    
        [Then(@"it should not match")]
        public void ThenItShouldNotMatch(Table table)
        {
            var collection = _scenarioContext["Collection"] as IEnumerable<Item>;
            Assert.IsFalse(table.RowCount == collection.Count() && table.ToProjection<Item>().Except(collection.ToProjection()).Count() == 0);
        }
    
        [Then(@"it should not exactly match")]
        public void ThenItShouldNotExactlyMatch(Table table)
        {
            var collection = _scenarioContext["Collection"] as IEnumerable<Item>;
            Assert.IsFalse(table.ToProjection<Item>().SequenceEqual(collection.ToProjection()));
        }
    }
```

In a similar way we can implement containment validation:

``` gherkin
    Scenario: Containment
    When I have a collection
    | Artist | Album |
    | Beatles | Rubber Soul |
    | Pink Floyd | Animals |
    | Muse | Absolution |
    Then it should contain all items
    | Artist | Album |
    | Beatles | Rubber Soul |
    | Muse | Absolution |
    But it should not contain all items
    | Artist | Album |
    | Beatles | Rubber Soul |
    | Muse | Resistance |
    And it should not contain any of items
    | Artist | Album |
    | Beatles | Abbey Road |
    | Muse | Resistance |
```

``` csharp
    [Binding]
    public class Binding
    {
        ScenarioContext _scenarioContext;

        public Binding(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Then(@"it should contain all items")]
        public void ThenItShouldContainAllItems(Table table)
        {
            var collection = _scenarioContext["Collection"] as IEnumerable<Item>;
            Assert.IsTrue(table.ToProjection<Item>().Except(collection.ToProjection()).Count() == 0);
        }

        [Then(@"it should not contain all items")]
        public void ThenItShouldNotContainAllItems(Table table)
        {
            var collection = _scenarioContext["Collection"] as IEnumerable<Item>;
            Assert.IsFalse(table.ToProjection<Item>().Except(collection.ToProjection()).Count() == 0);
        }

        [Then(@"it should not contain any of items")]
        public void ThenItShouldNotContainAnyOfItems(Table table)
        {
            var collection = _scenarioContext["Collection"] as IEnumerable<Item>;
            Assert.IsTrue(table.ToProjection<Item>().Except(collection.ToProjection()).Count() == table.RowCount);
        }
    }
```

What if Artist and Album are properties of different entities? Look at this piece of code:

``` csharp
    var collection = from x in ctx.Artists
                 where x.Name == "Muse"
                 join y in ctx.Albums on
                 x.ArtistId equals y.ArtistId
                 select new
                 {
                     Artist = x.Name,
                     Album = y.Name
                 };
```

**SpecFlow.Assist** has a generic class **EnumerableProjection<T>**. If a type “T” is known at compile time, **ToProjection** method converts a table or a collection straight to an instance of **EnumerableProjection**:

``` csharp
    table.ToProjection<Item>();
```

But if we need to compare a table with the collection of anonymous types from the example above, we need to express this type in some way so ToProjection will be able to build an instance of specialized **EnumerableProjection**. This is done by sending a collection as an argument to **ToProjection**. And to support both sets and instances and avoid naming ambiguity, corresponding methods are called **ToProjectionOfSet** and **ToProjectionOfInstance**:

``` csharp
    table.ToProjectionOfSet(collection);
    table.ToProjectionOfInstance(instance);
```

Here are the definitions of SpecFlow Table extensions methods that convert tables and collections of IEnumerables to EnumerableProjection:

``` csharp
    public static IEnumerable<Projection<T>> ToProjection<T>(this IEnumerable<T> collection, Table table = null)
    {
        return new EnumerableProjection<T>(table, collection);
    }
 
    public static IEnumerable<Projection<T>> ToProjection<T>(this Table table)
    {
        return new EnumerableProjection<T>(table);
    }
 
    public static IEnumerable<Projection<T>> ToProjectionOfSet<T>(this Table table, IEnumerable<T> collection)
    {
        return new EnumerableProjection<T>(table);
    }
 
    public static IEnumerable<Projection<T>> ToProjectionOfInstance<T>(this Table table, T instance)
    {
        return new EnumerableProjection<T>(table);
    }
```

Note that last arguments of **ToProjectionOfSet** and **ToProjectionOfInstance** methods are not used in method implementation. Their only purpose is to bring information about “T”, so the **EnumerableProjection** adapter class can be built properly. Now we can perform the following comparisons with anonymous types collections and instances:

``` csharp
    [Test]
    public void Table_with_subset_of_columns_with_matching_values_should_match_collection()
    {
        var table = CreateTableWithSubsetOfColumns();
        table.AddRow(1.ToString(), "a");
        table.AddRow(2.ToString(), "b");
 
        var query = from x in testCollection
                select new { x.GuidProperty, x.IntProperty, x.StringProperty };
 
        Assert.AreEqual(0, table.ToProjectionOfSet(query).Except(query.ToProjection()).Count());
    }
 
    [Test]
    public void Table_with_subset_of_columns_should_be_equal_to_matching_instance()
    {
        var table = CreateTableWithSubsetOfColumns();
        table.AddRow(1.ToString(), "a");
 
        var instance = new { IntProperty = testInstance.IntProperty, StringProperty = testInstance.StringProperty };
 
        Assert.AreEqual(table.ToProjectionOfInstance(instance), instance);
    }
```
