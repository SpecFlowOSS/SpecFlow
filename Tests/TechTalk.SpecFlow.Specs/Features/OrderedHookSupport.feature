Feature: Execute hooks in specified order
	As a developer
	I would like to be able to control the order of execution of hooks of the same type
	So that I can set up and teardown my scenario accordingly

Scenario Outline: Should execute 'Before*' SpecFlow events in the correct order, ie lowest numbers first
	Given a scenario 'Simple Scenario' as
         """
	     When I do something
         """
	And a hook 'FirstHook' for '<eventType>' with priority '1000'
	And a hook 'ThirdHook' for '<eventType>' with priority '3000'
	And a hook 'SecondHook' for '<eventType>' with priority '2000'
	And a hook 'DefaultHook' for '<eventType>'
	And a hook 'FifthHook' for '<eventType>' with priority '10001'
	And all steps are bound and pass
	When I execute the tests
	Then the hooks are executed in the order
	   | event       |
	   | FirstHook   |
	   | SecondHook  |
	   | ThirdHook   |
	   | DefaultHook |
	   | FifthHook   |

Examples:
    | eventType           |
    | Before      |
    | BeforeScenario      |
    | BeforeTestRun       |
    | BeforeFeature       |
    | BeforeScenarioBlock |
    | BeforeStep          |

Scenario Outline: Should execute 'After*' SpecFlow events in the correct order, ie highest numbers first
	Given a scenario 'Simple Scenario' as
         """
	     When I do something
         """
	And a hook 'FirstHook' for '<eventType>' with priority '1000'
	And a hook 'ThirdHook' for '<eventType>' with priority '3000'
	And a hook 'SecondHook' for '<eventType>' with priority '2000'
	And a hook 'DefaultHook' for '<eventType>'
	And a hook 'FifthHook' for '<eventType>' with priority '10001'
	And all steps are bound and pass
	When I execute the tests
	Then the hooks are executed in the order
	   | event       |
	   | FifthHook   |
	   | DefaultHook |
	   | ThirdHook   |
	   | SecondHook  |
	   | FirstHook   |

Examples:
    | eventType          |
    | After              |
    | AfterScenario      |
    | AfterFeature       |
    | AfterScenarioBlock |
    | AfterStep          |
	#Nunit bug, doesn't run after test run
    #| AfterTestRun       |