# Calling Steps from Step Definitions

**Note: This feature will be deprecated with SpecFlow 3.1 and removed in a future version (probably 4.0).**

It is possible to call steps from within [Step Definitions](Step Definitions.md):

``` csharp
[Binding]
public class CallingStepsFromStepDefinitionSteps : Steps
{
  [Given(@"the user (.*) exists")]
  public void GivenTheUserExists(string name)
  {
    // ...
  }

  [Given(@"I log in as (.*)")]
  public void GivenILogInAs(string name)
  {
    // ...
  }

  [Given(@"(.*) is logged in")]
  public void GivenIsLoggedIn(string name)
  {
    Given(string.Format("the user {0} exists", name));
    Given(string.Format("I log in as {0}", name));
  }
}
```

Invoking steps from step definitions is practical if you have several common steps that you want to perform in several scenarios, or simply if you want to make your scenarios shorter and more declarative. This allows you to do the following in a Scenario:

<pre>
# feature
Scenario: View last incidents
  Given Linda is logged in # This will in fact invoke 2 step definitions
  When I go to the incident page
</pre>

Instead of having a lot of repetition:

<pre>
# feature
Scenario: View last incidents
  Given the user Linda exists
  And I log in as Linda
  When I go to the incident page
</pre>

**Note:** When using this approach to remove duplications from your feature files, the console output will contain both the master step and the delegated steps as follows:

<pre>
  Given Linda is logged in
  Given the user Linda exists
  And I log in as Linda
  When I go to the incident page
</pre>

## Calling steps with multiline step arguments  

Sometimes you want to call a step that has been designed to take [Multiline Step Arguments](../Gherkin/Using Gherkin Language in SpecFlow.md), for example:

### Tables

```c#
[Given(@"an expense report for (.*) with the following posts:")]
public void GivenAnExpenseReportForWithTheFollowingPosts(string date, Table postTable)
{
  // ...
}
```

This can easily be called from a plain text step like this:

<pre>
# feature
Given an expense report for Jan 2009 with the following posts:
  | account | description | amount |
  | INT-100 | Taxi        |    114 |
  | CUC-101 | Peeler      |     22 |
</pre>

But what if you want to call this from a step definition? There are a couple of ways to do this:

```c#
[Given(@"A simple expense report")]
public void GivenASimpleExpenseReport()
{
    string[] header = {"account" , "description", "amount"};
    string[] row1 = {"INT-100" , "Taxi", "114"};
    string[] row2 = {"CUC-101" , "Peeler", "22"};
    var t = new Table(header);
    t.AddRow(row1);
    t.AddRow(row2);
    Given("an expense report for Jan 2009 with the following posts:", t);
}
```
