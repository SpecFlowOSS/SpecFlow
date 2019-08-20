Feature: DescriptionAccessing

Scenario: Check Scenario description is not empty
	Given the following binding class
        """
		using System;
		using TechTalk.SpecFlow;

		[Binding]
		public class DescriptionTestsBinding
		{
		    private readonly ScenarioContext _scenarioContext;

			public DescriptionTestsBinding(ScenarioContext scenarioContext)
			{
				_scenarioContext = scenarioContext;
			}

			[Then(@"Check ""(.*)"" match with scenario description in context")]
			public void ThenCheckMatchWithScenarioDescriptionInContext(string desc)
			{
				var testValue = _scenarioContext.ScenarioInfo.Description;
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
		    private readonly ScenarioContext _scenarioContext;

			public DescriptionTestsBinding(ScenarioContext scenarioContext)
			{
				_scenarioContext = scenarioContext;
			}

			[Then(@"Check that scenario description is null in context")]
			public void ThenCheckThatScenarioDescriptionIsNullInContext()
			{
				var testValue = _scenarioContext.ScenarioInfo.Description;
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


Scenario: Check Feature description is null if empty
	Given the following binding class
		"""
		using System;
		using TechTalk.SpecFlow;

		[Binding]
		public class StepsWithFeatureContext
		{
			private readonly FeatureContext featureContext;

			public StepsWithFeatureContext(FeatureContext featureContext)
			{
				if (featureContext == null) throw new ArgumentNullException("featureContext");
				this.featureContext = featureContext;
			}

			[Then(@"Check that Feature description is null in context")]
			public void ThenCheckThatFeatureDescriptionIsNullInContext()
			{
				var testValue = featureContext.FeatureInfo.Description;;
				if (testValue != null) throw new Exception("Feature Description is incorrectly parsed"); 						
			}

		}
        """	

	And there is a feature file in the project as
         """
		 Feature: DescriptionFeature
		 
		 Scenario: FeatureDescriptionCheck	 
		 Then Check that Feature description is null in context
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |


Scenario: Check Feature description is not empty
	Given the following binding class
		"""
		using System;
		using TechTalk.SpecFlow;

		[Binding]
		public class StepsWithFeatureContext
		{
			private readonly FeatureContext featureContext;

			public StepsWithFeatureContext(FeatureContext featureContext)
			{
				if (featureContext == null) throw new ArgumentNullException("featureContext");
				this.featureContext = featureContext;
			}

			
			[Then(@"Check ""(.*)"" match with Feature description in context")]
			public void ThenCheckMatchWithFeatureDescriptionInContext(string desc)
			{
				var testValue = featureContext.FeatureInfo.Description;;
				if (testValue != desc) throw new Exception("Feature Description is incorrectly parsed"); 	
			}
		}
        """	

	And there is a feature file in the project as
         """
		 Feature: DescriptionFeature
		 Test Feature Description

		 Scenario: FeatureDescriptionCheck	 
		 Then Check "Test Feature Description" match with Feature description in context
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |









