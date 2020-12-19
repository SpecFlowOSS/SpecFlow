# Value Retrievers

SpecFlow can turn properties in a table like this:

```gherkin
Given I have the following people
| First Name | Last Name | Age | IsAdmin | 
| John       | Guppy     | 40  | true    |
```

Into an object like this:

```c#
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public bool IsAdmin { get; set; }
}
```

With commands like these:

```c#
    [Given(@"I have the following people")]
    public void x(Table table)
    {
        var person = table.CreateInstance<Person>();
        // OR
        var people = table.CreateSet<Person>();
    }

```

But how does SpecFlow match the values in the table with the values in the object?  It does so with Value Retrievers.  There are value retrievers defined for almost every C# base type, so mapping most basic POCOs can be done with SpecFlow without any modification.

## Extending with your own value retrievers

Often you might have a more complicated POCO type, one that is not comprised solely of C# base types.  Like this one:

```c#
    public class Shirt
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
    }

    public class Color
    {
        public string Name { get; set; }
    }
```
 
Simple example how to process the human readable color 'red' to the Hex value:

```gherkin
| First Name | ShirtColor | 
| Scott      | Red        |
```
The table will be processed, and the following code can be used to capture the table translation and customize it:
```c#
public class ShirtColorValueRetriever : IValueRetriever
    {
        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
           if (!keyValuePair.Key.Equals("ShirtColor"))
           {
               return false;
           }

           bool value;
           if (Color.TryParse(keyValuePair.Value, out value))
           {
              return true;
           }  
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return Color.Parse(keyValuePair.Value).HexCode;
        }
    }
```

## Registering Custom ValueRetrievers

Before you can utilize a custom `ValueRetriever`, you'll need to register it. We recommend doing this prior to a test run using the `[BeforeTestRun]` attribute and `Service.Instance.ValueRetrievers.Registr()`. For example:

``` csharp
[Binding]
public static class Hooks
{
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Service.Instance.ValueRetrievers.Register(new MyCustomValueRetriever());
    }
}
```
