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
            Console.WriteLine("Starting " + scenarioContext.ScenarioInfo.Title);
        }

        [AfterScenario]
        public void AfterScenario(ScenarioContext scenarioContext)
        {
            Console.WriteLine("Finished " + scenarioContext.ScenarioInfo.Title);
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
            Console.WriteLine("Starting " + featureContext.FeatureInfo.Title);
        }
    
        [AfterFeature]
        public static void AfterFeature(FeatureContext featureContext)
        {
            Console.WriteLine("Finished " + featureContext.FeatureInfo.Title);
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
            //All parameters are resolved from the test thread container automatically.
            //Since the global container is the base container of the test thread container, globally registered services can be also injected.
        
            //ITestRunManager from global container
            var location = testRunnerManager.TestAssembly.Location;
            
            //ITestRunner from test thread container
            var threadId = testRunner.ThreadId;
        }
            
        [AfterTestRun]
        public static void AfterTestRun(ITestRunnerManager testRunnerManager, ITestRunner testRunner)
        {
            //ITestRunManager from global container
            var location = testRunnerManager.TestAssembly.Location;
            
            //ITestRunner from test thread container
            var threadId = testRunner.ThreadId;
        }
        """
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |
    
    
    