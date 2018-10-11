Feature: StepDefinitionReport


Scenario: All steps are bound with one step

Given there is a SpecFlow project
And there is a feature file in the project as
	"""
			Feature: Simple Feature
			Scenario: Simple Scenario
				Given there is something
				When I do something
				Then something should happen
		"""
And the following step definitions
		 """
			[Given("there is something")]
			public void GivenThereIsSomething()
			{}

			[When("I do something")]
			public void WhenIDoSomething()
			{}

			[Then("something should happen")]
			public void ThenSomethingShouldHappen()
			{}
		 """

When the project is compiled
And I generate SpecFlow Step Definition report

Then the generated report contains
	"""
	Givens Step Definition Instances there is something [copy] 1 [show] Instances: there is something [copy] Simple Feature / Simple Scenario
	Whens Step Definition Instances I do something [copy] 1 [show] Instances: I do something [copy] Simple Feature / Simple Scenario
	Thens Step Definition Instances something should happen [copy] 1 [show] Instances: something should happen [copy] Simple Feature / Simple Scenario
	"""

Scenario: One step with two bindings

Given there is a SpecFlow project
And there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario: Simple Scenario
				Given there is something
		"""
And the following step definitions
		 """
			[Given("there is something")]
			public void GivenThereIsSomething()
			{}

			[Given("there is something")]
			public void GivenThereIsSomething_Duplicate()
			{}

		 """

When the project is compiled
And I generate SpecFlow Step Definition report

Then the generated report contains
	"""
	Givens Step Definition Instances
	there is something [copy] 1 [show] Instances:
	there is something [copy] Simple Feature / Simple Scenario
	there is something [copy] 0
	Whens Step Definition Instances
	Thens Step Definition Instances
	"""


Scenario: Multiple usage of same step

Given there is a SpecFlow project
And there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario: Simple Scenario
				Given there is something

			Scenario: Simple Scenario 2
				Given there is something
		"""
And the following step definitions
		 """
			[Given("there is something")]
			public void GivenThereIsSomething()
			{}
		 """

When the project is compiled
And I generate SpecFlow Step Definition report

Then the generated report contains
	"""
	Givens Step Definition Instances
	there is something [copy] 2 [show] Instances:
	there is something [copy] Simple Feature / Simple Scenario Simple Feature / Simple Scenario 2
	Whens Step Definition Instances
	Thens Step Definition Instances
	"""

Scenario: Report can handle scenario outlines

Given there is a SpecFlow project
And there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario Outline: Simple Scenario Outline
				Given there is something

			Examples:
				| Col 1     |
				| Example 1 |
				| Example 2 |

		"""
And the following step definitions
		 """
			[Given("there is something")]
			public void GivenThereIsSomething()
			{}
		 """

When the project is compiled
And I generate SpecFlow Step Definition report

Then the generated report contains
	"""
	Givens Step Definition Instances
	there is something [copy] 1 [show] Instances:
	there is something [copy] Simple Feature / Simple Scenario Outline (Scenario Outline Example)
	Whens Step Definition Instances
	Thens Step Definition Instances
	"""

Scenario: Report can handle scenario outlines with variables in step text

Given there is a SpecFlow project
And there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario Outline: Simple Scenario Outline
				Given there is <Col 1>

			Examples:
				| Col 1     |
				| Example 1 |
				| Example 2 |

		"""
And the following step definitions
		 """
			[Given("there is Example 1")]
			public void GivenThereIsExample1(string magic)
			{}

			[Given("there is Example 2")]
			public void GivenThereIsExample2()
			{}
		 """

When the project is compiled
And I generate SpecFlow Step Definition report

Then the generated report contains
	"""
	Givens Step Definition Instances
	there is Example 1 [copy] 1 [show] Instances:
	there is Example 1 [copy] Simple Feature / Simple Scenario Outline (Scenario Outline Example)
	there is Example 2 [copy] 1 [show] Instances:
	there is Example 2 [copy] Simple Feature / Simple Scenario Outline (Scenario Outline Example)
	Whens Step Definition Instances
	Thens Step Definition Instances
	"""

Scenario: Report can handle data tables

Given there is a SpecFlow project
And there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario: Simple Scenario
				Given there is something
					| Column 1 |
					| Value 1  |
					| Value 2  |

		"""
And the following step definitions
		 """
			[Given("there is something")]
			public void GivenThereIsSomething(Table table)
			{}
		 """

When the project is compiled
And I generate SpecFlow Step Definition report

Then the generated report contains
	"""
	Givens Step Definition Instances
	there is something [copy] 1 [show] Instances:
	there is something [copy] Simple Feature / Simple Scenario
	Whens Step Definition Instances
	Thens Step Definition Instances
	"""

Scenario: Report can handle doc strings

Given there is a SpecFlow project
And there is a feature file in the project as
		"""
			Feature: Simple Feature
			Scenario: Simple Scenario
				Given there is something
					'''
					DocString
					'''

		"""
And the following step definitions
		 """
			[Given("there is something")]
			public void GivenThereIsSomething(string docString)
			{}
		 """

When the project is compiled
And I generate SpecFlow Step Definition report

Then the generated report contains
	"""
	Givens Step Definition Instances
	there is something [copy] 1 [show] Instances:
	there is something [copy] Simple Feature / Simple Scenario
	Whens Step Definition Instances
	Thens Step Definition Instances
	"""
