Feature: Scenario Description Accessing

Scenario: Check Scenario description is not empty
	Given the following binding class
        """
		using System;
		using TechTalk.SpecFlow;

		[Binding]
		public class DescriptionTestsBinding
		{
			[Then(@"Check ""(.*)"" match with scenario description in context")]
			public void ThenCheckMatchWithScenarioDescriptionInContext(string desc)
			{
				var testValue = ScenarioContext.Current.ScenarioInfo.Description;
				if (testValue != desc) throw new Exception("Scenario Description is incorrectly parsed"); 						 
			}
		}
        """	

	And there is a feature file in the project as
         """
		 Feature: DescriptionFeature
		 Test Feature Description

		 Scenario: ScenarioDescriptionCheck
		 Test Scenario Description
		 Then Check "Test Scenario Description" match with scenario description in context
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: Check Scenario description is null if empty
	Given the following binding class
        """
		using System;
		using TechTalk.SpecFlow;

		[Binding]
		public class DescriptionTestsBinding
		{	
			[Then(@"Check that scenario description is null in context")]
			public void ThenCheckThatScenarioDescriptionIsNullInContext()
			{
				var testValue = ScenarioContext.Current.ScenarioInfo.Description;
				if (testValue != null) throw new Exception("Scenario Description is incorrectly parsed"); 						 
			}
		}
        """	

	And there is a feature file in the project as
         """
		 Feature: DescriptionFeature
		 Test Feature Description

		 Scenario: ScenarioDescriptionCheck
		 
		 Then Check that scenario description is null in context
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |
