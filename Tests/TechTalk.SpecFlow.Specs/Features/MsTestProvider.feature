@mstest
Feature: MsTest unit test provider

Scenario Outline: Should be able to execute scenarios with basic results
	Given there is a SpecFlow project
	And the project is configured to use the MsTest provider
	And a scenario 'Simple Scenario' as
		"""
		When I do something
		"""
	And all steps are bound and <step definition status>
	When I execute the tests with MsTest
	Then the execution summary should contain
		| Total | <result> |
		| 1     | 1        |

Examples: 
	| result    | step definition status |
	| Succeeded | pass                   |
	| Failed    | fail                   |

Scenario: Should handle scenario outlines
	Given there is a SpecFlow project
	And the project is configured to use the MsTest provider
	Given there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario Outline: Simple Scenario Outline
				Given there is something
				When I do <what>
				Then something should happen
			Examples: 
				| what           |
				| something      |
				| something else |
		"""
	And all steps are bound and pass
	When I execute the tests with MsTest
	Then the execution summary should contain
		| Succeeded |
		| 2         |

Scenario: Should be able to access TestContext in Steps
Given there is a SpecFlow project
And the project is configured to use the MsTest provider
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
		 When I execute the tests with MsTest
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
	And the specflow configuration is
		"""
		<specFlow>
			<unitTestProvider name="MsTest"/>
		</specFlow>
		"""
	When I execute the tests with MsTest
	Then the execution summary should contain
		| Total | 
		| 1     | 
	
@config
Scenario: Should be able to deploy files
    Given there is a SpecFlow project
	And the following binding class
        """
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
	And the specflow configuration is
		"""
		<specFlow>
			<unitTestProvider name="MsTest"/>
		</specFlow>
		"""
	And there is a test settings file 'Local.testsettings'
		"""
		<?xml version="1.0" encoding="UTF-8"?>
		<TestSettings
		  id="b8f2810b-cd53-4519-8b18-d0e599219d54"
		  name="Local"
		  enableDefaultDataCollectors="false"
		  xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
		  <Deployment enabled="true" />
		</TestSettings>
		"""
	When I execute the tests with MsTest
	Then the execution summary should contain
         | Succeeded |
         | 1         |

@config
Scenario: Should be able to deploy files to specific folder
    Given there is a SpecFlow project
	And the following binding class
        """
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
	And the specflow configuration is
		"""
		<specFlow>
			<unitTestProvider name="MsTest"/>
		</specFlow>
		"""
	And there is a test settings file 'Local.testsettings'
		"""
		<?xml version="1.0" encoding="UTF-8"?>
		<TestSettings
		  id="b8f2810b-cd53-4519-8b18-d0e599219d54"
		  name="Local"
		  enableDefaultDataCollectors="false"
		  xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
		  <Deployment enabled="true" />
		</TestSettings>
		"""
	When I execute the tests with MsTest
	Then the execution summary should contain
         | Succeeded |
         | 1         |