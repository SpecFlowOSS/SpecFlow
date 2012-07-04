Feature: Execute hooks
	As a developer
	I would like to be able to hook into pre and post conditions in SpecFlow
	So that I can set up and teardown my scenario accordingly

Scenario Outline: Should execute SpecFlow events
	Given a scenario 'Simple Scenario' as
         """
	     When I do something
         """
	And a hook 'HookFor<event>' for '<event>'
	And all steps are bound and pass
	When I execute the tests
	Then the hook 'HookFor<event>' is executed once

Examples: 
	| event               |
	| BeforeScenario      |
	| AfterScenario       |
	| BeforeFeature       |
	| AfterFeature        |
	| BeforeStep          |
	| AfterStep           |
	| BeforeScenarioBlock |
	| AfterScenarioBlock  |
	| BeforeTestRun       |
#bug: nunit does not run the AfterTestRun event
#	| AfterTestRun        |

Examples: Cucumber compatibility
	| event  |
	| Before |
	| After  |

Scenario Outline: Should execute the hooks according to their semantics
	Given there is a feature file in the project as
        """
		Feature: Feature 1
		
		Scenario: Scenario 1
		Given there is something
		Given there is something
		When I do something
		When I do something
		Then something should happen
		Then something should happen
		
		Scenario: Scenario 2
		Given there is something
		Given there is something
		When I do something
		When I do something
		Then something should happen
		Then something should happen
        """
	Given there is a feature file in the project as
        """
		Feature: Feature 2
		
		Scenario: Scenario 3
		Given there is something
		Given there is something
		When I do something
		When I do something
		Then something should happen
		Then something should happen
        """
	And a hook 'HookFor<event>' for '<event>'
	And all steps are bound and pass
	When I execute the tests
	Then the hook 'HookFor<event>' is executed <count> times

Examples: 
	| event               | count |
	| BeforeStep          | 18    |
	| AfterStep           | 18    |
	| BeforeScenarioBlock | 9     |
	| AfterScenarioBlock  | 9     |
	| BeforeScenario      | 3     |
	| AfterScenario       | 3     |
	| BeforeFeature       | 2     |
	| AfterFeature        | 2     |
	| BeforeTestRun       | 1     |
#bug: nunit does not run the AfterTestRun event
#	| AfterTestRun        | 1     |

Examples: Cucumber compatibility
	| event  | count |
	| Before | 3     |
	| After  | 3     |

