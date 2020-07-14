# Step Argument Conversions

[Step bindings](Step-Definitions.md) can use parameters to make them reusable for similar steps. The parameters are taken from either the step's text or from the values in additional examples. These arguments are provided as either strings or `TechTalk.SpecFlow.Table` instances.

To avoid cumbersome conversions in the step binding methods, SpecFlow can perform an automatic conversion from the arguments to the parameter type in the binding method. All conversions are performed using the culture of the feature file, unless the [bindingCulture element](../Installation/Configuration.md) is defined in your `app.config` file (see [Feature Language](../Gherkin/Feature-Language.md)). The following conversions can be performed by SpecFlow (in the following precedence):

* no conversion, if the argument is an instance of the parameter type (e.g. the parameter type is `object` or `string`)
* step argument transformation
* standard conversion

## Step Argument Transformation

Step argument transformations can be used to apply a custom conversion step to the arguments in step definitions. The step argument transformation is a method that converts from text (specified by a regular expression) or a `Table` instance to an arbitrary .NET type.

A step argument transformation is used to convert an argument if:

* The return type of the transformation is the same as the parameter type
* The regular expression (if specified) matches the original (string) argument

**Note:** If multiple matching transformation are available, a warning is output in the trace and the first transformation is used.

The following example transforms a relative period of time (`in 3 days`) into a `DateTime` structure.

```c#
[Binding]
public class Transforms
{
    [StepArgumentTransformation(@"in (\d+) days?")]
    public DateTime InXDaysTransform(int days)
   {
      return DateTime.Today.AddDays(days);
   }
}
```

The following example transforms any string input (no regex provided) into an `XmlDocument`.

```c#
[Binding]
public class Transforms
{
    [StepArgumentTransformation]
    public XmlDocument XmlTransform(string xml)
    {
       XmlDocument result = new XmlDocument();
       result.LoadXml(xml);
       return result;
    }
}
```

The following example transforms a table argument into a list of `Book` entities (using the [SpecFlow Assist Helpers](SpecFlow-Assist-Helpers.md)).  

```c#
[Binding]
public class Transforms
{
    [StepArgumentTransformation]
    public IEnumerable<Book> BooksTransform(Table booksTable)
    {
       return booksTable.CreateSet<Books>();
    }
}
```

```c#
[Binding]
public class Transforms
{
  [Given(@"Show messenger""(.*)""")]
  public void GiveShowMessenger()
  {
    string chave = nfe.Tab();
    Assert.IsNotNull(chave);
  }
}
```

## Standard Conversion

A standard conversion is performed by SpecFlow in the following cases:

* The argument can be converted to the parameter type using `Convert.ChangeType()`
* The parameter type is an `enum` type and the (string) argument is an enum value
* The parameter type is `Guid` and the argument contains a full GUID string or a GUID string prefix. In the latter case, the value is filled with trailing zeroes.
