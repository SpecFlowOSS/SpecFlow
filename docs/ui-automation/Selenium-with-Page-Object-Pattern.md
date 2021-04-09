# Selenium with Page Object Model Pattern

Selenium is a free (open-source) automation framework used for web applications across different browsers and platforms, you can read more about them [here](https://www.selenium.dev/). It is often used in connection with SpecFlow to test your web application via their user interface.

The Page Object Model Pattern is an often used pattern to abstract your page into separate classes. With it you have your element selectors at dedicated locations and not scattered around your automation code.

## Sample Project Setup

You can download this entire sample project from [Github](https://github.com/SpecFlowOSS/SpecFlow-Examples/tree/master/CalculatorSelenium).

Base of this sample project is the default project that is created from the SpecFlow project template.

Additional used NuGet package to the standard packages:

- [Selenium.Support](https://www.nuget.org/packages/Selenium.Support/3.141.0) - Main package for Selenium
- [Selenium.WebDriver.ChromeDriver](https://www.nuget.org/packages/Selenium.WebDriver.ChromeDriver/89.0.4389.2300) - Package that contains the ChromeDriver so Selenium is able to control the Chrome browser

## Sample Scenario

The web application we are testing in this example is a simple calculator implementation hosted [here](https://specflowoss.github.io/Calculator-Demo/Calculator.html). Feel free to use this for practice if you like to.

We are testing the web application by simply adding two numbers together and checking the results.

In order to test more than just the two initial numbers in the default feature file from the project template we have added an extra Scenario Outline with the parameters `First number`, `Second number`, and `Expected result`. Now we can use an example table to include as many numbers as we like.

Here is a snippet of the feature file:

*Calculator.feature*

``` gherkin
Scenario: Add two numbers
	Given the first number is 50
	And the second number is 70
	When the two numbers are added
	Then the result should be 120


Scenario Outline: Add two numbers permutations
	Given the first number is <First number>
	And the second number is <Second number>
	When the two numbers are added
	Then the result should be <Expected result>

Examples:
	| First number | Second number | Expected result |
	| 0            | 0             | 0               |
	| -1           | 10            | 9               |
	| 6            | 9             | 15              |
```

## Starting and quiting the Browser

We start with configuring the browser behavior, the opening and closing of Google Chrome for our tests:

This class handles it for us. When you access the `Current` property the first time, the browser will be opened. If we did this, the browser will be automatically closed after the scenario finished.

*BrowserDriver.cs*

``` csharp

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CalculatorSelenium.Specs.Drivers
{
    /// <summary>
    /// Manages a browser instance using Selenium
    /// </summary>
    public class BrowserDriver : IDisposable
    {
        private readonly Lazy<IWebDriver> _currentWebDriverLazy;
        private bool _isDisposed;

        public BrowserDriver()
        {
            _currentWebDriverLazy = new Lazy<IWebDriver>(CreateWebDriver);
        }

        /// <summary>
        /// The Selenium IWebDriver instance
        /// </summary>
        public IWebDriver Current => _currentWebDriverLazy.Value;

        /// <summary>
        /// Creates the Selenium web driver (opens a browser)
        /// </summary>
        /// <returns></returns>
        private IWebDriver CreateWebDriver()
        {
            //We use the Chrome browser
            var chromeDriverService = ChromeDriverService.CreateDefaultService();

            var chromeOptions = new ChromeOptions();

            var chromeDriver = new ChromeDriver(chromeDriverService, chromeOptions);

            return chromeDriver;
        }

        /// <summary>
        /// Disposes the Selenium web driver (closing the browser) after the Scenario completed
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            if (_currentWebDriverLazy.IsValueCreated)
            {
                Current.Quit();
            }

            _isDisposed = true;
        }
    }
}
```

## Using Page Objects

Since we are using Page Object Model Pattern we are **not** adding our UI automation directly in bindings.

Using the Selenium WebDriver we simulate a user interacting with the webpage. The element IDs on the page are used to identify the fields we want to enter data into. Other functions here are basically simulating a user entering numbers into the calculator, adding them up, waiting for results, and moving on to the next test.

The code is well commented so you can understand what each line is for:

*CalculatorPageObject.cs*

```csharp
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CalculatorSelenium.Specs.PageObjects
{
    /// <summary>
    /// Calculator Page Object
    /// </summary>
    public class CalculatorPageObject
    {
        //The URL of the calculator to be opened in the browser
        private const string CalculatorUrl = "https://specflowoss.github.io/Calculator-Demo/Calculator.html";

        //The Selenium web driver to automate the browser
        private readonly IWebDriver _webDriver;
        
        //The default wait time in seconds for wait.Until
        public const int DefaultWaitInSeconds = 5;

        public CalculatorPageObject(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        //Finding elements by ID
        private IWebElement FirstNumberElement => _webDriver.FindElement(By.Id("first-number"));
        private IWebElement SecondNumberElement => _webDriver.FindElement(By.Id("second-number"));
        private IWebElement AddButtonElement => _webDriver.FindElement(By.Id("add-button"));
        private IWebElement ResultElement => _webDriver.FindElement(By.Id("result"));
        private IWebElement ResetButtonElement => _webDriver.FindElement(By.Id("reset-button"));

        public void EnterFirstNumber(string number)
        {
            //Clear text box
            FirstNumberElement.Clear();
            //Enter text
            FirstNumberElement.SendKeys(number);
        }

        public void EnterSecondNumber(string number)
        {
            //Clear text box
            SecondNumberElement.Clear();
            //Enter text
            SecondNumberElement.SendKeys(number);
        }

        public void ClickAdd()
        {
            //Click the add button
            AddButtonElement.Click();
        }

        public void EnsureCalculatorIsOpenAndReset()
        {
            //Open the calculator page in the browser if not opened yet
            if (_webDriver.Url != CalculatorUrl)
            {
                _webDriver.Url = CalculatorUrl;
            }
            //Otherwise reset the calculator by clicking the reset button
            else
            {
                //Click the rest button
                ResetButtonElement.Click();

                //Wait until the result is empty again
                WaitForEmptyResult();
            }
        }

        public string WaitForNonEmptyResult()
        {
            //Wait for the result to be not empty
            return WaitUntil(
                () => ResultElement.GetAttribute("value"),
                result => !string.IsNullOrEmpty(result));
        }

        public string WaitForEmptyResult()
        {
            //Wait for the result to be empty
            return WaitUntil(
                () => ResultElement.GetAttribute("value"),
                result => result == string.Empty);
        }

        /// <summary>
        /// Helper method to wait until the expected result is available on the UI
        /// </summary>
        /// <typeparam name="T">The type of result to retrieve</typeparam>
        /// <param name="getResult">The function to poll the result from the UI</param>
        /// <param name="isResultAccepted">The function to decide if the polled result is accepted</param>
        /// <returns>An accepted result returned from the UI. If the UI does not return an accepted result within the timeout an exception is thrown.</returns>
        private T WaitUntil<T>(Func<T> getResult, Func<T, bool> isResultAccepted) where T: class
        {
            var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(DefaultWaitInSeconds));
            return wait.Until(driver =>
            {
                var result = getResult();
                if (!isResultAccepted(result))
                    return default;

                return result;
            });
        }
    }
}
```

Here is the code of the step definition file. Note the usage of the *calculatorPageObject* and *Browserdriver*.

*CalculatorStepDefinitions.cs*

```csharp
using CalculatorSelenium.Specs.Drivers;
using CalculatorSelenium.Specs.PageObjects;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace CalculatorSelenium.Specs.Steps
{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {
        //Page Object for Calculator
        private readonly CalculatorPageObject _calculatorPageObject;

        public CalculatorStepDefinitions(BrowserDriver browserDriver)
        {
            _calculatorPageObject = new CalculatorPageObject(browserDriver.Current);
        }

        [Given("the first number is (.*)")]
        public void GivenTheFirstNumberIs(int number)
        {
            //delegate to Page Object
            _calculatorPageObject.EnterFirstNumber(number.ToString());
        }

        [Given("the second number is (.*)")]
        public void GivenTheSecondNumberIs(int number)
        {
            //delegate to Page Object
            _calculatorPageObject.EnterSecondNumber(number.ToString());
        }

        [When("the two numbers are added")]
        public void WhenTheTwoNumbersAreAdded()
        {
            //delegate to Page Object
            _calculatorPageObject.ClickAdd();
        }

        [Then("the result should be (.*)")]
        public void ThenTheResultShouldBe(int expectedResult)
        {
            //delegate to Page Object
            var actualResult = _calculatorPageObject.WaitForNonEmptyResult();

            actualResult.Should().Be(expectedResult.ToString());
        }
    }
}
```

**> Note:** The `Then` step here is the "testing" part where we compare the results from the Page Object Model Pattern with the expected results. As there is a time delay between hitting the add button and getting the result, we need to handle this behavior with the `WaitForNonEmptyResult()` method.

## Using the same browser for all scenarios

 In order to avoid having multiple browsers opened up during the test run and save some time per scenario, we use the **same** browser to run all the tests. For that we have introduced the below [Hook](../Bindings/Hooks.md). The major trade off here is you lose the ability to run test in parallel since you are using a single browser instance.

*SharedBrowserHooks.cs*

```csharp
using BoDi;
using CalculatorSelenium.Specs.Drivers;
using TechTalk.SpecFlow;

namespace CalculatorSelenium.Specs.Hooks
{
    /// <summary>
    /// Share the same browser window for all scenarios
    /// </summary>
    /// <remarks>
    /// This makes the sequential execution of scenarios faster (opening a new browser window each time would take more time)
    /// As a tradeoff:
    ///  - we cannot run the tests in parallel
    ///  - we have to "reset" the state of the browser before each scenario
    /// </remarks>
    [Binding]
    public class SharedBrowserHooks
    {
        [BeforeTestRun]
        public static void BeforeTestRun(ObjectContainer testThreadContainer)
        {
            //Initialize a shared BrowserDriver in the global container
            testThreadContainer.BaseContainer.Resolve<BrowserDriver>();
        }
    }
}
```

If you don't want this, simply delete the class.

## Resetting the Web Application

Because we reuse the browser instance, we have to reset the web app for every scenario. We are using again a hook to do this.

*CalculatorHooks.cs*

```csharp
using CalculatorSelenium.Specs.Drivers;
using CalculatorSelenium.Specs.PageObjects;
using TechTalk.SpecFlow;

namespace CalculatorSelenium.Specs.Hooks
{
    /// <summary>
    /// Calculator related hooks
    /// </summary>
    [Binding]
    public class CalculatorHooks
    {
        ///<summary>
        ///  Reset the calculator before each scenario tagged with "Calculator"
        /// </summary>
        [BeforeScenario("Calculator")]
        public static void BeforeScenario(BrowserDriver browserDriver)
        {
            var calculatorPageObject = new CalculatorPageObject(browserDriver.Current);
            calculatorPageObject.EnsureCalculatorIsOpenAndReset();
        }
    }
}
```

## Further Reading

If you want to get into more details, have a look at the following documentation pages:

- [Hooks](../Bindings/Hooks.md)  
  All about Hooks, the lifecycle events in SpecFlow
- [Driver Pattern](../Guides/DriverPattern.md)  
  More details and examples about the Driver Pattern, which we used for the Browser Lifecycle handling
- [Page Object Model Pattern](../Guides/PageObjectModel.md)  
  More details and examples for the Page Object Model Patter
