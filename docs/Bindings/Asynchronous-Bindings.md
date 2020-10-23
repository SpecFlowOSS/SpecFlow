# Asynchronous Bindings

If you have code that executes an [asynchronous task](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/index), you can define asynchronous bindings to execute the corresponding code using the `async` and `await` keywords.

A sample project using asynchronous bindings can be found [here](https://github.com/techtalk/SpecFlow-Examples/tree/master/AsyncAwait). The `When` binding in [WebSteps.cs](https://github.com/techtalk/SpecFlow-Examples/blob/master/AsyncAwait/WebRequest.Specs/StepDefinitions/WebSteps.cs#L24) is asynchronous, and defined as follows:

``` csharp
[When(@"I want to get the web page '(.*)'")]
public async Task WhenIWantToGetTheWebPage(string url)
{
    await _webDriver.HttpClientGet(url);
}
```

The HTTPClientGet asynchronous task is defined in [WebDriver.cs](https://github.com/techtalk/SpecFlow-Examples/blob/master/AsyncAwait/WebRequest.Specs/Drivers/WebDriver.cs#L17) as follows:

``` csharp
public async Task HttpClientGet(string url)
{
    _httpResponseMessage = await _httpClient.GetAsync(url);
}
```
