@MSTest
Feature: TestContext

Scenario: Should be able to access TestContext in Steps
    Given there is a SpecFlow project
    And a scenario 'Simple Scenario' as
        """
        When I do something
        """	
    And the following step definition
        """
        [When(@"I do something")]
        public void WhenIDoSomething()
        {
        System.Console.WriteLine(ScenarioContext.Current.ScenarioContainer.Resolve<Microsoft.VisualStudio.TestTools.UnitTesting.TestContext>().TestName);
        }
        """
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |
    
Scenario: Should be able to access TestContext in BeforeTestRun/AfterTestRun hooks
    Given there is a SpecFlow project
    And there is a scenario	
    And all steps are bound and pass
    And the following hooks
        """
        [BeforeTestRun]
        public static void BeforeTestRun(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testContext.WriteLine(testContext.TestRunDirectory);
        }

        [AfterTestRun]
        public static void AfterTestRun(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testContext.WriteLine(testContext.TestRunDirectory);
        }
        """
    When I execute the tests
    Then the execution summary should contain
        | Succeeded |
        | 1         |