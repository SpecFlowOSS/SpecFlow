@MSTest
Feature: DeploymentItem
    
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
