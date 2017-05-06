Feature: Configuration
	
Background: 
	Given there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario: Simple Scenario
				Given there is something
				When I do something
				Then something should happen
		"""
	And all steps are bound and pass


Scenario: Generation configuration in app.config
	Given SpecFlow is configured in the app.config
	When I execute the tests
	Then the app.config is used for configuration

Scenario: Generation configuration in specflow.json
	Given SpecFlow is configured in the specflow.json
	When I execute the tests
	Then the specflow.json is used for configuration	

Scenario: Runtime configuration in app.config
	Given SpecFlow is configured in the app.config
	When I execute the tests
	Then the app.config is used for configuration

Scenario: Runtime configuration in specflow.json
	Given SpecFlow is configured in the specflow.json
	When I execute the tests
	Then the specflow.json is used for configuration	

