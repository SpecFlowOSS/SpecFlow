Feature: Scenario Tags Accessing

Scenario: Accessing tags of a simple scenario
	Given the following step definitions
		"""
		[When("the scenario tags are accessed from a step definition")]
		public void WhenTheScenarioTagsAreAccessedFromAStepDefinition()
		{
			if (!_scenarioContext.ScenarioInfo.Tags.Contains("scenario_tag")) throw new Exception("Scenario Tags does not contain scenario tags"); 						 
			if (!_scenarioContext.ScenarioInfo.CombinedTags.Contains("scenario_tag")) throw new Exception("Scenario CombinedTags does not contain scenario tags"); 						 
			if (!_scenarioContext.ScenarioInfo.CombinedTags.Contains("feature_tag")) throw new Exception("Scenario CombinedTags does not contain feature tags");
		}
        """	

	And there is a feature file in the project as
         """
		 @feature_tag
		 Feature: Sample feature

		 @scenario_tag
		 Scenario: Sample scenario
		 When the scenario tags are accessed from a step definition
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: Accessing tags of a scenario outline
	Given the following step definitions
		"""
		[When("the scenario tags are accessed from a step definition")]
		public void WhenTheScenarioTagsAreAccessedFromAStepDefinition()
		{
			if (!_scenarioContext.ScenarioInfo.Tags.Contains("scenario_tag")) throw new Exception("Scenario Tags does not contain scenario tags");
			if (!_scenarioContext.ScenarioInfo.Tags.Contains("examples_tag")) throw new Exception("Scenario Tags does not contain example block tags");
			if (!_scenarioContext.ScenarioInfo.CombinedTags.Contains("scenario_tag")) throw new Exception("Scenario CombinedTags does not contain scenario tags"); 						 
			if (!_scenarioContext.ScenarioInfo.CombinedTags.Contains("feature_tag")) throw new Exception("Scenario CombinedTags does not contain feature tags");
			if (!_scenarioContext.ScenarioInfo.CombinedTags.Contains("examples_tag")) throw new Exception("Scenario CombinedTags does not contain example block tags");
		}
        """	

	And there is a feature file in the project as
         """
		 @feature_tag
		 Feature: Sample feature

		 @scenario_tag
		 Scenario Outline: Sample scenario outline
		 When the scenario tags are accessed from a step definition
		 @examples_tag
		 Examples:
		   | nr |
		   | 1  |
		   | 2  |
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 2         |

Scenario: Accessing tags of a scenario inside a rule
	Given the following step definitions
		"""
		[When("the scenario tags are accessed from a step definition")]
		public void WhenTheScenarioTagsAreAccessedFromAStepDefinition()
		{
			if (!_scenarioContext.ScenarioInfo.Tags.Contains("scenario_tag")) throw new Exception("Scenario Tags does not contain scenario tags"); 						 
			if (!_scenarioContext.ScenarioInfo.CombinedTags.Contains("scenario_tag")) throw new Exception("Scenario CombinedTags does not contain scenario tags"); 						 
			if (!_scenarioContext.ScenarioInfo.CombinedTags.Contains("feature_tag")) throw new Exception("Scenario CombinedTags does not contain feature tags");
			if (!_scenarioContext.ScenarioInfo.CombinedTags.Contains("rule_tag")) throw new Exception("Scenario CombinedTags does not contain rule tags");
		}
        """	

	And there is a feature file in the project as
         """
		 @feature_tag
		 Feature: Sample feature

		 @rule_tag
		 Rule: Sample rule

		 @scenario_tag
		 Scenario: Sample scenario
		 When the scenario tags are accessed from a step definition
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |
