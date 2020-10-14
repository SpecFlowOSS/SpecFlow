# Using Page Object Model with Selenium

The Page Object Model is a pattern, that is often used to abstract your Web UI with Selenium to easier automate it.

So to automate following HTML snippet

``` html
<input id="txtUrl" name="Url" type="text" value="">
```

you have following class to control it

``` csharp
public class PageObject
{
    public IWebElement TxtUrl {get;}
}
```

When you are working with Selenium, you are always working with WebElements to access the different elements on your Website. You can find them with the `FindElement` and `FindElements` methods on the `WebDriver` class.  
If you are always using these methods directly in your automation code, you will get a lot of code duplication. This is the moment when you should start using the Page Object Model.
You hide the calls to the `FindElement(s)` methods in a class.

This has following advantages:

- the classes are easier reusable
- if you need to change an `id` of your element, you need to change only one place
- your bindings are less dependent on your HTML structure

## Simple Implementation

**HTML**:

``` html
<input id="txtUrl" name="Url" type="text" value="">
```

**Code**:

``` csharp
public class PageObject
{
    private IWebDriver _webDriver;

    public PageObject(IWebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    public IWebElement txtUrl => _webDriver.FindElement(By.Id("txtUrl"));
}
```

You pass your `WebDriver` instance via constructor, and always when you access the `TxtUrl` property, the `WebDriver` searches on the whole page for an element with the id `txtUrl`. There is no caching involved.

## Implementation with Caching

**HTML**:

``` html
<input id="txtUrl" name="Url" type="text" value="">
```

**Code**:

``` csharp
public class PageObject
{
    private IWebDriver _webDriver;
    private Lazy<IWebElement> _txtUrl;

    public PageObject(IWebDriver webDriver)
    {
        _webDriver = webDriver;
        _txtUrl = new Lazy<IWebElement>(() => _webDriver.FindElement(By.Id("txtUrl")));
    }

    public IWebElement txtUrl => _txtUrl.Value;
}
```

Again You pass your `WebDriver` instance via constructor. In this case we are using [Lazy](https://docs.microsoft.com/en-us/dotnet/api/system.lazy-1) as a easy way to cache the result of the `FindElement` method.  
Only the first call to the `txtUrl` property, triggers a call to the `FindElement` function. All subsequent calls, will return the same value as before. 
This will save you some time in execution of your automation code, as the WebDriver needs to do search less often for the same element.

If you use a caching strategy like that, be careful with your lifetime of your page objects and your page. Don't reuse an old instance of your page model, if the page changed in the meantime.

## Implementation with Hierarchy

**HTML**:

``` html
<div class='A'>
    <div class='B'/>
</div>
<div class='B'>
</div>
```

**Code**:

``` csharp
public class ParentPageObject
{
    private IWebDriver _webDriver;

    public ParentPageObject(IWebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    public IWebElement WebElement => _webDriver.FindElement(By.ClassName("A"));

    public ChildPageObject Child => new ChildPageObject(WebElement);
}

public class ChildPageObject
{
    private IWebElement _webElement;
    private Lazy<IWebElement> _txtUrl;

    public ChildPageObject(IWebElement webElement)
    {
        _webElement = webElement;
    }

    public IWebElement WebElement => _webElement.FindElement(By.ClassName("B"));
}

```

In this example we have a slightly adjusted HTML document to work with. There are two `div`- elements with the same class `B`, but we only want the PageObject for the `div`- element with the class `A` and the child.  

If we would use the same `WebDriver.FindElement` method we would get the `div`- element that is on the same level as the `A` div.  
But every WebElement has also the `FindElement(s)`- methods. This enable you to query the elements only in a part of your whole HTML DOM.  
To do that  we are passing this time the `parent`- WebElement to the `ChildPageObject` class to only search for the element with the class `B` within the `A`- div.  

This concept enables you to structure your PageObjects in a similar way you have your HTML DOM structure.

## Further resources

- <https://www.browserstack.com/guide/page-object-model-in-selenium>
- <https://www.selenium.dev/documentation/en/guidelines_and_recommendations/page_object_models/>
- <https://martinfowler.com/bliki/PageObject.html>
