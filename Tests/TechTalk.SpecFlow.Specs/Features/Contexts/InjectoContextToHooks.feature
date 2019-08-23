@wip_gn
Feature: Injecting context into hooks

Background: 
  Given a scenario 'Simple Scenario' as
    """
    When I do something
    """
  And all steps are bound and pass



Scenario Outline: Inject FeatureContext into a Feature hook
  Given the following hook
    """
    [<Hook>]
    public static void <Hook>Hook(FeatureContext featureContext)
    {
        if (featureContext == null)
        {
            throw new ArgumentNullException(nameof(featureContext));
        }

        global::Log.LogHook();
    }
    """
  When I execute the tests
  Then all tests should pass
  And the hook '<Hook>Hook' is executed once

Examples: 
  | Hook          |
  | BeforeFeature |
  | AfterFeature  |


Scenario Outline: Inject ScenarioContext into a Scenario hook
  Given the following hook
    """
    [<Hook>]
    public void <Hook>Hook(ScenarioContext scenarioContext)
    {
        if (scenarioContext == null)
        {
            throw new ArgumentNullException(nameof(scenarioContext));
        }

        global::Log.LogHook();
    }
    """
  When I execute the tests
  Then all tests should pass
  And the hook '<Hook>Hook' is executed once

Examples: 
  | Hook           |
  | BeforeScenario |
  | AfterScenario  |
