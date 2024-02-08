# Driver Pattern

Over the years, we've noticed that a good practice for organizing bindings and automation code is to keep the code in bindings short (around 10 lines) and easy to understand. The Driver Pattern can be used to achieve this by providing additional layers between bindings and the actual automation code. These "additional layers" provide reusable methods and scenario-specific data contexts that can be used in a variety of test scenarios.

Using a Driver Pattern in your test set up provides the following benefits:

**1. Prevent Duplicate Code**
In your application there's a good chance some steps that are performed many times over. For example, if you are building a website that requires users to login, it's likely that most interactions will begin with a login and the Driver Pattern can reduce duplicate code.

**2. Easier Code Maintenance**

As business requirements change, the test setup should be able to quickly change with it. Continuing with the login user flow example from the previous point, if the login process changes to include multi-factor authentication, the Driver Pattern enables developers to change code in one area which may impact all tests.

In the context of web development, at some point you will accidentally write code that results in one or more flaky tests. When using the Driver Pattern, fixing flaky test code in a method shared by multiple bindings may fix all your tests with a single change.

**3. Easy Method Re-Use and Grouping** 
It's quite common to see a group of steps that are frequently used together in test scenarios. The driver pattern allows users to group up method calls for easier code reusability.

**4. Easier to Read Step Definitions** 
By using points 1-3, non-technical stakeholders should also be able to understand step definitions as business logic is captured in methods with names reflecting actions a user takes in an application.

The Driver pattern is heavily using [Context- Injection](../Bindings/Context-Injection.md) to connect the multiple classes together.

## Example

In this example you see how the code looks before and after refactoring with the Driver pattern.

**Before**:

This is some automation code that uses the [Page Object Model](PageObjectModel.md) and checks if some WebElements are existing.  

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

    public SubmissionSteps(SubmissionPageDriver submissionPageDriver)
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

## Further Resources

- <http://leitner.io/2015/11/14/driver-pattern-empowers-your-specflow-step-definitions>
