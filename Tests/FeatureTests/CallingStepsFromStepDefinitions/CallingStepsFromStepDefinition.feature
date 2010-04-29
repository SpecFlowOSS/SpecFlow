Feature: Calling Steps from StepDefinitions
	In order to create steps of a higher abstraction
	As a developer
	I want reuse other steps in my step definitions

Scenario: Log in
	Given I am on the index page
	When I enter my unsername nad password
	And I click the login button
	Then the welcome page should be displayed

Scenario: Do something meaningful
	Given I am logged in
	When I dosomething meaningful
	Then I should get rewarded