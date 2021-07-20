# External Data Plugin

You can easily apply standardize test cases across a wide range of features to significantly reduce
redundant data for large test suites, speed up exploratory and approval testing by reusing execution flows
for ranges of examples. SpecFlow makes all this possible by introducing support for loading external data into
scenarios easily.

The [SpecFlow ExternalData plugin](https://www.nuget.org/packages/SpecFlow.ExternalData/3.4.32-beta) lets teams separate test data from test scenarios, and reuse examples across a large set of scenarios. This is particularly helpful when a common set of examples needs to be consistently verified in different scenarios.

## Examples

### Improving Test Coverage

There are usually two classes of examples for scenarios in the Given-When-Then format.

The first set is driven primarily with the domain-specific scenario context. For example, examples involving various time zones are important for end-of-day reports. These examples are usually critically important to show in the scenario itself, to provide shared understanding and a good context for development.

The second set is driven purely by data formats, regardless of the specific scenario. For example, leap second and leap year tests are usually good for something involving dates. Developers will often forget to check for those edge cases, so testers usually need to check for them on any date-related scenarios. This second set of examples usually does not improve shared understanding, but still needs to be tested. Adding it to all scenarios involving dates creates a lot of redundancy and noise, and actually distracts from shared understanding.

With the new ExternalData plugin, you can now showcase important examples for a scenario, and simply mix in all the other examples without creating noise or redundancy. The following snippet will check for the key examples listed below the scenario, but also check all external examples listed in a database of valid emails.

```Gherkin
@property:email=E-mail_addresses.Valid
Scenario Outline: recording user information on successful registration

Given a visitor registering as "Mike Scott" with email <email>
When the registration completes
Then the account system should record <email> related to user "Mike Scott"

Examples: key examples
  | Variant            | email              |
  | simple valid email | simple@example.com |
```

Note the `@property` tag at the top of the scenario. That’s the only important change from the usual way of specifying scenarios with placeholders. Add this tag, and specflow will know how to map a scenario placeholder `<email>` to a database of valid emails in various formats.

### Manage Standardized Tests Easily

Another important use case for the ExternalData plugin is to improve consistency across scenarios, for example to test payment workflows for all supported currencies easily. Previously, you had to copy and paste the same currency list in many places, which was laborious and error prone. With the ExternalData plugin, this task becomes as easy as writing a simple scenario outline. Save the external data into a configuration file, then map it to scenario parameters using the `@property` tag. Here’s a simple example:

```Gherkin
@property:currency=Valid_Currencies
Scenario: New forex contract for valid currency
  Given a forex contract for <currency> 
  When the contract is received
  Then the contract status should be "Open"
```

Note that this scenario has a placeholder `<currency>`, but the list of examples is not specified below the scenario. Instead, the currencies will be loaded from an external file attached to the project.

You can then easily introduce other scenarios for the same currencies:

```Gherkin
@property:currency=Valid_Currencies
Scenario: same-currency payments do not trigger forex
  Given a payment of 100 <currency> 
  And a related account account in <currency>
  When the payment is received
  Then the forex rate is 1.00
```

Managing a list of supported currencies in a single place ensures that team members do not unintentionally forget an important currency when testing. It also allows you to quickly introduce new supported currencies into the test suite without having to edit individual scenarios.

### Support Exploratory Testing Easily

Using this plugin, testers can create temporary scenarios to quickly explore the system, and use SpecFlow as a way to run through a bunch of cases easily, or automate data setups for additional manual testing. For example, testers can create a set of interesting sample text blocks, with various length and special characters, and then quickly check their features against just by including the reference in a temporary scenario.

```Gherkin
@property:text=Lorems
Scenario: comment wrapping
Given a comment with text <text>
And the screen height of 400px
When the comment box is shown 
Then the the comment size should not exceed 2 lines
```

Such scenarios can be easily modified depending on test results to check additional cases, and testers can re-run the whole set quickly. Important discoveries from exploratory testing sessions could be later integrated into the main test suite.

### Use Synthetic Test Data

The ExternalData plugin lets organizations publish and share important synthetic test data across teams, or even integrate publicly available test data sets into their projects. This is particularly useful for teams that do not have dedicated testers or people who can come up with good boundaries for exploring the system. The first version of the plugin already comes with support for [BugMagnet](https://github.com/gojko/bugmagnet) configuration file formats, so you can easily integrate test cases from the standard BugMagnet database, or any of the additional configuration files published by the community. Here is an example that uses the list of email edge cases from the BugMagnet database to check if a system rejects invalid emails correctly:

```Gherkin
@property:email=E-mail_addresses.Invalid
Scenario: rejecting invalid emails

Given a visitor registering as "Mike Scott" with email <email>
When the registration completes
Then the account system should not record <email> 
And the error response should be "Invalid Email"
```

### More information

This plugin was inspired by community feedback from Given-When-Then With Style challenge, in particular the solution for *[How to deal with groups of similar scenarios?](https://specflow.org/blog/solving-how-to-deal-with-groups-of-similar-scenarios-givenwhenthenwithstyle/)* The old solution works well only for relatively small sets of data, and it was difficult to share across different projects. Instead of technical hacks and workarounds, you can now use the ExternalData plugin for a systematic solution to data sharing, and to let testers and developers manage larger sets of test data easily.

Use the following two links to get started easily:

- [Download the plugin](https://www.nuget.org/packages/SpecFlow.ExternalData/3.4.32-beta)

- Check out some nice [usage examples](https://github.com/SpecFlowOSS/SpecFlow-Examples/tree/master/ExternalDataSample) (including the BugMagnet database integration)
