# Step Argument Transformations

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

The following example transforms a table argument into a list of `Book` entities (using the [[SpecFlow Assist Helpers]]). 

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