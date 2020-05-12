@scenario_info_arguments
Feature: Scenario Arguments Accessing

Scenario: Scenario arguments are empty for regular scenario
	Given the following binding class
        """
		using System;
		using TechTalk.SpecFlow;

		[Binding]
		public class A
		{
			[Then("B")]
			public void B()
			{
				if (ScenarioContext.Current.ScenarioInfo.Arguments.Count != 0) throw new Exception("Scenario arguments are not empty");
			}
		}
        """	

	And there is a feature file in the project as
         """
		 Feature: A

		 Scenario: B
		 Then B
         """

	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: Scenario arguments for outlined scenario
	Given the following binding class
        """
		using System;
		using TechTalk.SpecFlow;

		[Binding]
		public class A
		{
			[When("B")]
			public void B()
			{
				
			}

			[BeforeScenario("tag_a")]
			public void BeforeTagA(ScenarioContext scenarioContext)
			{
				if (scenarioContext.ScenarioInfo.Arguments["column 1"].ToString() != "value 1") throw new Exception($"Actual: '{scenarioContext.ScenarioInfo.Arguments["column 1"].ToString()}'");
				if (scenarioContext.ScenarioInfo.Arguments["column 2"].ToString() != "value 2") throw new Exception($"Actual: '{scenarioContext.ScenarioInfo.Arguments["column 2"].ToString()}'");
			}

			[BeforeScenario("tag_b")]
			public void BeforeTagB(ScenarioContext scenarioContext)
			{
				if (scenarioContext.ScenarioInfo.Arguments["column 1"].ToString() != "value 11") throw new Exception($"Actual: '{scenarioContext.ScenarioInfo.Arguments["column 1"].ToString()}'");
				if (scenarioContext.ScenarioInfo.Arguments["column 2"].ToString() != "value 22") throw new Exception($"Actual: '{scenarioContext.ScenarioInfo.Arguments["column 2"].ToString()}'");
			}
		}
        """	

	And there is a feature file in the project as
         """
		 Feature: A

		 Scenario Outline: B
		 When B
		 @tag_a
		 Examples:
		 	| column 1 	| column 2 	|
		 	| value 1 	| value 2 	|
		 @tag_b
		 Examples:
		 	| column 1 	| column 2 	|
		 	| value 11 	| value 22 	|
         """

	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 2         |