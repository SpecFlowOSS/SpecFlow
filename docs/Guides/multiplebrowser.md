# Targeting Multiple Browser with a Single Test

If you are testing a web app (e.g. with Selenium), you will normally want to test it in a range of browsers, e.g. Chrome, IE/Edge and Firefox. However, writing tests for all the browsers can be a time-consuming process. It would be much easier to just write just one test and run that test in all browsers.

[SpecFlow+ Runner](https://specflow.org/tools/runner/) allows you to do this by using [Targets](https://docs.specflow.org/projects/specflow-runner/en/latest/Profile/Targets.html). Targets are defined in your SpecFlow+ Runner profile. They allow you to define different environment settings, filters and deployment transformation steps for each target. Another common use case is to define separate targets for X64 and x86.

Defining targets for each browser allows us to execute the same test in all browsers. You can see this in action in the Selenium sample project available on [GitHub](https://github.com/SpecFlowOSS/SpecFlow.Plus.Examples/tree/master/SeleniumWebTest). If you download the solution and open [Default.srprofile](https://github.com/SpecFlowOSS/SpecFlow.Plus.Examples/blob/master/SeleniumWebTest/TestApplication.UiTests/Default.srprofile#L29), you will see 3 different targets defined at the end of the file:

``` csharp

<Targets>
  <Target name="IE">
    <Filter>Browser_IE</Filter>
    <DeploymentTransformationSteps>
      <EnvironmentVariable variable="Test_Browser" value="IE" />
    </DeploymentTransformationSteps>
  </Target>
  <Target name="Chrome">
    <Filter>Browser_Chrome</Filter>
    <DeploymentTransformationSteps>
      <EnvironmentVariable variable="Test_Browser" value="Chrome" />
    </DeploymentTransformationSteps>
  </Target>
  <Target name="Firefox">
    <Filter>Browser_Firefox</Filter>
    <DeploymentTransformationSteps>
      <EnvironmentVariable variable="Test_Browser" value="Firefox" />
    </DeploymentTransformationSteps>
  </Target>
</Targets>

```

Each of the targets has a name and an associated filter (e.g. “Browser_IE”). The filter ensures that only tests with the corresponding tag are executed for that target.

For each target, we [transform](https://msdn.microsoft.com/en-us/library/dd465326(v=vs.110).aspx) the `Test_browser` environment variable to contain the name of the target. This will allow us to know the current target and access the corresponding web driver for each browser. [WebDriver.cs](https://github.com/techtalk/SpecFlow.Plus.Examples/blob/master/SeleniumWebTest/TestApplication.UiTests/Drivers/WebDriver.cs) (located in the *Drivers* folder of the *TestApplication.UiTests* project) uses this key to instantiate a web driver of the appropriate type (e.g. *InternetExplorerDriver*). Based on the value of this environment variable, the appropriate web driver is returned by `GetWebDriver()` is passed to BrowserConfig, used in the switch statement:

``` csharp

private IWebDriver GetWebDriver()
{
  switch (Environment.GetEnvironmentVariable("Test_Browser"))
  {
    case "IE": return new InternetExplorerDriver(new InternetExplorerOptions { IgnoreZoomLevel = true }) { Url = SeleniumBaseUrl };
    case "Chrome": return new ChromeDriver { Url = SeleniumBaseUrl };
    case "Firefox": return new FirefoxDriver { Url = SeleniumBaseUrl };
    case string browser: throw new NotSupportedException($"{browser} is not a supported browser");
    default: throw new NotSupportedException("not supported browser: <null>");
  }
}

```

Depending on the target, the driver is instantiated as either the *InternetExplorerDriver*, *ChromeDriver* or *FirefoxDriver* driver type. The bindings code simply uses the required web driver for the target to execute the test; there is no need to write separate tests for each browser. You can see this at work in the [Browser.cs](https://github.com/techtalk/SpecFlow.Plus.Examples/blob/master/SeleniumWebTest/TestApplication.UiTests/Steps/Browser.cs) and [CalculatorFeatureSteps.cs](https://github.com/techtalk/SpecFlow.Plus.Examples/blob/master/SeleniumWebTest/TestApplication.UiTests/Steps/CalculatorFeatureSteps.cs#L8) files:

``` csharp

[Binding]
public class CalculatorFeatureSteps
{
  private readonly WebDriver _webDriver;

  public CalculatorFeatureSteps(WebDriver webDriver)
  {
    _webDriver = webDriver;
  }
       
  [Given(@"I have entered (.*) into (.*) calculator")]
  public void GivenIHaveEnteredIntoTheCalculator(int p0, string id)
  {
    _webDriver.Wait.Until(d => d.FindElement(By.Id(id))).SendKeys(p0.ToString());
  }

  ```

To ensure that the tests are executed, you still need to ensure that the tests have the appropriate tags *(@Browser_Chrome, @Browser_IE, @Browser_Firefox)*. 2 scenarios have been defined in [CalculatorFeature.feature](https://github.com/techtalk/SpecFlow.Plus.Examples/blob/master/SeleniumWebTest/TestApplication.UiTests/Features/CalculatorFeature.feature):

``` Gherkin

@Browser_Chrome
@Browser_IE
@Browser_Firefox
Scenario: Basepage is Calculator
	Given I navigated to /
	Then browser title is Calculator

@Browser_IE 
@Browser_Chrome
Scenario Outline: Add Two Numbers
	Given I navigated to /
	And I have entered <SummandOne> into summandOne calculator
	And I have entered <SummandTwo> into summandTwo calculator
	When I press add
	Then the result should be <Result> on the screen

Scenarios: 
		| SummandOne | SummandTwo | Result |       
		| 50         | 70         | 120    | 
		| 1          | 10         | 11     |
    
```

Using targets in this way can significantly reduce the number of tests you need to write and maintain. You can reuse the same test and bindings for multiple browsers. Once you’ve set up your targets and web driver, all you need to do is tag your scenarios correctly. If you select “Traits” under **Group By Project** in the Test Explorer, the tests are split up by browser tag. You can easily run a test in a particular browser and identify which browser the tests failed in. The test report generated by SpecFlow+ Runner also splits up the test results by target/browser.

Remember that targets can be used for a lot more than executing the same test in multiple browsers with Selenium. Don’t forget to read the documentation on [targets](https://specflow.org/plus/documentation/targets/), as well as the sections on [filters](https://specflow.org/plus/documentation/Filter/), [target environments](https://specflow.org/plus/documentation/Environment/) and [deployment transformations](https://specflow.org/plus/documentation/DeploymentTransformation/).
