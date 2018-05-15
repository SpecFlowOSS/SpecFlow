@SingleTestConfiguration
Feature: DescriptionParser

Scenario: Parsing of Feature Description
	Given there is a Gherkin file as
	"""
		Feature: DescriptionParser
		Test Feature Description
	"""
	When the file is parsed
	Then no parsing error is reported

Scenario: Parsing of Scenario Description
	Given there is a Gherkin file as
	"""
		Feature: DescriptionParser

		Scenario: ScenarioDescriptionParse
		Test Scenario Description
			Given something
			When something 
	"""
	When the file is parsed
	Then no parsing error is reported

Scenario: Parsing of Scenario and Feature Description
	Given there is a Gherkin file as
	"""
		Feature: DescriptionParser
		Test Feature Description
		Scenario: ScenarioDescriptionParse
		Test Scenario Description
			Given something
			When something 
	"""
	When the file is parsed
	Then no parsing error is reported



