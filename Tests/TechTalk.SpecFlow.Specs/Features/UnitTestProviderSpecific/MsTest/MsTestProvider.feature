@mstest
Feature: MsTest unit test provider

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

@config
Scenario: Should be able to specify MsTest provider in the configuration
    Given there is a SpecFlow project
    And a scenario 'Simple Scenario' as
        """
        When I do something
        """
    And all steps are bound and pass
    When I execute the tests
    Then the execution summary should contain
        | Total | 
        | 1     | 
    
@config
Scenario: Should be able to deploy files
    Given there is a SpecFlow project
    And the following binding class
        """
        using System;
        using System.IO;
        using TechTalk.SpecFlow;

        using Microsoft.VisualStudio.TestTools.UnitTesting;

        [Binding]
        public class DeploymentItemSteps
        {
            [Then(@"the file '(.*)' exists")]
            public void ThenTheFileExists(string fileName)
            {
                Assert.IsTrue(File.Exists(fileName));
            }
        }
        """
    And there is a feature file in the project as
         """
         @MsTest:DeploymentItem:DeploymentItemTestFile.txt
         Feature: Deployment Item Feature
    
         Scenario: Deployment Item Scenario
            Then the file 'DeploymentItemTestFile.txt' exists
         """
    And there is a content file 'DeploymentItemTestFile.txt' in the project as
        """
        This is a deployment item file
        """
    When I execute the tests
    Then the execution summary should contain
         | Succeeded |
         | 1         |

@config
Scenario: Should be able to deploy files to specific folder
    Given there is a SpecFlow project
    And the following binding class
        """
        using System;
        using System.IO;
        using TechTalk.SpecFlow;

        using Microsoft.VisualStudio.TestTools.UnitTesting;

        [Binding]
        public class DeploymentItemSteps
        {
            [Then(@"the file '(.*)' exists")]
            public void ThenTheFileExists(string fileName)
            {
                Assert.IsTrue(File.Exists(fileName));
            }
        }
        """
    And there is a feature file in the project as
         """
         @MsTest:DeploymentItem:Resources\DeploymentItemTestFile.txt:Data
         Feature: Deployment Item Feature
    
         Scenario: Deployment Item Scenario
            Then the file 'Data\DeploymentItemTestFile.txt' exists
         """
    And there is a content file 'Resources\DeploymentItemTestFile.txt' in the project as
        """
        This is a deployment item file
        """
    When I execute the tests
    Then the execution summary should contain
         | Succeeded |
         | 1         |