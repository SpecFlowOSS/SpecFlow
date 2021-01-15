Feature: Inject hook parameters
    As a developer
    I would like to get my hook parameters automatically injected
    So that I can use the appropriate FeatureContext/ScenarioContext or other registered services in my hook implementation

Scenario: Should be able to access ScenarioContext in BeforeScenario/AfterScenario hooks as parameter
Note: the parameters of the BeforeScenario/AfterScenario hook are resolved from the scenario container
    Given there is a SpecFlow project
    And there is a scenario	
    And all steps are bound and pass
    And the following hooks
        """
        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
            if (scenarioContext == null) throw new Exception("ScenarioContext wasn't passed correctly as parameter to BeforeScenario");
        }

        [AfterScenario]
        public void AfterScenario(ScenarioContext scenarioContext)
        {
            if (scenarioContext == null) throw new Exception("ScenarioContext wasn't passed correctly as parameter to AfterScenario");
        }
        """
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |
    
Scenario: Should be able to access FeatureContext in BeforeFeature/AfterFeature hooks as parameter
Note: the parameters of the BeforeFeature/AfterFeature hook are resolved from the feature container
    Given there is a SpecFlow project
    And there is a scenario	
    And all steps are bound and pass
    And the following hooks
        """
        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            if (featureContext == null) throw new Exception("FeatureContext wasn't passed correctly as parameter to BeforeFeature");
        }
    
        [AfterFeature]
        public static void AfterFeature(FeatureContext featureContext)
        {
            if (featureContext == null) throw new Exception("FeatureContext wasn't passed correctly as parameter to AfterFeature");
        }        
        """
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |
    
Scenario: Should be able to access thread level and global services in BeforeTestRun/AfterTestRun hooks as parameter
Note: the parameters of the BeforeFeature/AfterFeature hook are resolved from the test thread container
    Given there is a SpecFlow project
    And there is a scenario	
    And all steps are bound and pass
    And the following hooks
        """
        [BeforeTestRun]
        public static void BeforeTestRun(ITestRunnerManager testRunnerManager, ITestRunner testRunner)
        {
            //All parameters are resolved from the test thread container automatically. ITestRunnerManager and ITestRunner are just some examples here.
            //Since the global container is the base container of the test thread container, globally registered services can be also injected.
        
            //ITestRunManager from global container
            if (testRunnerManager == null) throw new Exception("ITestRunManager wasn't passed correctly as parameter to BeforeTestRun");
            
            //ITestRunner from test thread container
            if (testRunner == null) throw new Exception("ITestRunner wasn't passed correctly as parameter to BeforeTestRun");
        }
            
        [AfterTestRun]
        public static void AfterTestRun(ITestRunnerManager testRunnerManager, ITestRunner testRunner)
        {
            //ITestRunManager from global container
            if (testRunnerManager == null) throw new Exception("ITestRunManager wasn't passed correctly as parameter to AfterTestRun");
            
            //ITestRunner from test thread container
            if (testRunner == null) throw new Exception("ITestRunner wasn't passed correctly as parameter to AfterTestRun");
        }
        """
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |
    
    
    