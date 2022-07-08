@cucumberExpressions
Feature: Cucumber expressions

Rule: Should support Cucumber expressions

Scenario: Cucumber expresions are used for Step Definitions
	Given a scenario 'Simple Scenario' as
         """
			When I have 42 cucumbers in my belly
			And there is a user 'Marvin' registered
         """
	And the following step definitions
        """
		[When("I have {int} cucumber(s) in my belly/tummy")]
		public void WhenIHaveCucumbersInMyBelly(int count)
		{
		    if (count == 42)
              global::Log.LogStep(); 
		}

		[When("there is a user {string} registered")]
		public void ThereIsAUserRegistered(string userName)
		{
		    if (userName == "Marvin")
              global::Log.LogStep(); 
		}
        """
	When I execute the tests
	Then the binding method 'WhenIHaveCucumbersInMyBelly' is executed
	And the binding method 'ThereIsAUserRegistered' is executed


Rule: Regular expressions and Cucumber expressions can be mixed

The expression is considered to be a regular expression if any of the statements is true

* Starts with `^`
* Ends with `$`
* Contains `.*`
* Contains `(<some expression>*)`
* Contains `(<some expression>+)`
* Contains `\d+`
* Contains `\.`

Except for `^` and `$` all other conditions are ignored if the expression contains a cucumber expression parameter placeholder (`{<parameter name>}`).


Scenario: Cucumber expresions and Regular expressions are mixed in the same project
	Given a scenario 'Simple Scenario' as
         """
			When I have 42 cucumbers in my belly
			And there is a user 'Marvin' registered
         """
	And the following step definitions
        """
		[When("I have {int} cucumbers in my belly")]
		public void WhenIHaveCucumbersInMyBelly(int count)
		{
		    if (count == 42)
              global::Log.LogStep(); 
		}

		[When(@"there is a user '(.*)' registered")]
		public void ThereIsAUserRegistered(string userName)
		{
		    if (userName == "Marvin")
              global::Log.LogStep(); 
		}
        """
	When I execute the tests
	Then the binding method 'WhenIHaveCucumbersInMyBelly' is executed
	And the binding method 'ThereIsAUserRegistered' is executed



Rule: Custom parameter types can be used in Cucumber expressions

Scenario: Custom parameter types definied with [StepArgumentTransformation] can be used with type name
	Given a scenario 'Simple Scenario' as
         """
			When I download the release v1.2.3 of the application
         """
	And the following step argument transformation
		"""
		[StepArgumentTransformation("v(.*)")]
		public Version ConvertVersion(string versionString)
		{
		    return new Version(versionString);
		}
		"""
	And the following step definition
        """
		[When("I download the release {Version} of the application")]
		public void WhenIDownloadTheReleaseOfTheApplication(Version version)
		{
		    if (version.ToString() == "1.2.3")
              global::Log.LogStep(); 
		}
        """
	When I execute the tests
	Then the binding method 'WhenIDownloadTheReleaseOfTheApplication' is executed

Scenario: Custom parameter types definied with [StepArgumentTransformation] can be used with specified name
	Given a scenario 'Simple Scenario' as
         """
			When I download the release v1.2.3 of the application
         """
	And the following step argument transformation
		"""
		[StepArgumentTransformation("v(.*)", Name = "my_version")]
		public Version ConvertVersion(string versionString)
		{
		    return new Version(versionString);
		}
		"""
	And the following step definition
        """
		[When("I download the release {my_version} of the application")]
		public void WhenIDownloadTheReleaseOfTheApplication(Version version)
		{
		    if (version.ToString() == "1.2.3")
              global::Log.LogStep(); 
		}
        """
	When I execute the tests
	Then the binding method 'WhenIDownloadTheReleaseOfTheApplication' is executed
