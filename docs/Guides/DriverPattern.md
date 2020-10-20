# Driver Pattern

Over the years, we noticed that a good practice to organize your bindings and automation code is to keep the code in the bindings very short (around 10 lines) and easy understandable.  
This makes it possible, that also non- technical people can understand what is happening in a step definition. This makes your live in bigger projects easier, because nobody will remember what every single step is doing.  

One of the best ways to achieve this is to use the driver pattern.

## What is the Driver Pattern? 

The Driver Pattern is simply an additional layer between your step definitions and your automation code.

Here is an example of how the code looks before and after refactoring with the Driver pattern.

**Before**:

This is some automation code that uses the [Page Object Model](PageObjectModel.md) and checks if some WebElements are existing. A person with development knowledge can understand what this code is doing after reading it. But a person without this knowledge will have a hard time to understand what is going on.

``` csharp
[Then(@"it is possible to enter a '(.*)' with label '(.*)'")]
public void ThenItIsPossibleToEnterAWithLabel(string inputType, string expectedLabel)
{
    var submissionPageObject = new SubmissionPageObject(webDriverDriver);

    switch (inputType.ToUpper())
    {
        case "URL":
            submissionPageObject.UrlWebElement.Should().NotBeNull();
            submissionPageObject.UrlLabel.Should().Be(expectedLabel);
            break;
        case "TYPE":
            submissionPageObject.TypeWebElement.Should().NotBeNull();
            submissionPageObject.TypeLabel.Should().Be(expectedLabel);
            break;
        default:
            throw new NotImplementedException(inputType + " not implemented");
    }
}
```

![Architecture before](../_static/images/DriverPattern_Before.png)

**After**:

With moving the automation code into a driver class, we could reduce the number of lines in the step definition to one. Also we can now use a method-name (`CheckExistenceOfInputElement`), that is understandable by everybody in your team.

To get an instance of the driver class (`SubmissionSteps`), we are using the [Context- Injection](../Bindings/Context-Injection.md) Feature of SpecFlow.

``` csharp
[Binding]
public class SubmissionSteps
{
    private readonly SubmissionPageDriver submissionPageDriver;

    public SubmissionSteps(SubmissionDriver submissionDriver)
    {
        this.submissionPageDriver = submissionPageDriver;
    }

    [Then(@"it is possible to enter a '(.*)' with label '(.*)'")]
    public void ThenItIsPossibleToEnterAWithLabel(string inputType, string expectedLabel)
    {
        submissionPageDriver.CheckExistenceOfInputElement(inputType, expectedLabel);
    }

    // ...
```

``` csharp
public class SubmissionPageDriver
{
    // ...

    public void CheckExistenceOfInputElement(string inputType, string expectedLabel)
    {
        var submissionPageObject = new SubmissionPageObject(webDriverDriver);

        switch (inputType.ToUpper())
        {
            case "URL":
                submissionPageObject.UrlWebElement.Should().NotBeNull();
                submissionPageObject.UrlLabel.Should().Be(expectedLabel);
                break;
            case "TYPE":
                submissionPageObject.TypeWebElement.Should().NotBeNull();
                submissionPageObject.TypeLabel.Should().Be(expectedLabel);
                break;
            default:
                throw new NotImplementedException(inputType + " not implemented");
        }
    }

    // ...
```

![Architecture after](../_static/images/DriverPattern_After.png)

## Benefits

With now the methods in a separate class, you get also following benefits

- easier to read step definitions
- easy to reuse methods in different step definitions
- easy to combine multiple steps into a single step
  
## Further Resources

- <http://leitner.io/2015/11/14/driver-pattern-empowers-your-specflow-step-definitions>
