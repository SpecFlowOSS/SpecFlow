Feature: Specifying the culture to be used for argument conversions

Scenario: The default culture is used for bindings if not specified
	Given a scenario 'Simple Scenario' as
         """
         Given I have entered 3.14 into the calculator
         """
	And the following step definition
         """
         [Given(@"I have entered (.+) into the calculator")]
		 public void GivenIHaveEntered(double number)
		 {
			if (number != 3.14) throw new Exception("number converted incorrectly");
		 }
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

Scenario: The scenario language is used for bindings if not specified
	Given there is a feature file in the project as
         """
		 #language:de
		 Funktionalität: Argument transformation
	
		 Szenario: Steps mit Argumenten
			Angenommen ich habe 3,14 in den Taschenrechner eingegeben
         """
	And the following step definition
         """
         [Given(@"ich habe (.+) in den Taschenrechner eingegeben")]
		 public void GivenIHaveEntered(double number)
		 {
			if (number != 3.14) throw new Exception("number converted incorrectly");
		 }
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |

@config
Scenario: The binding culture can be specified to be different than the scenario culture
	Given a scenario 'Simple Scenario' as
         """
         Given I have entered 3,14 into the calculator
         """
	And the following step definition
         """
         [Given(@"I have entered (.+) into the calculator")]
		 public void GivenIHaveEntered(double number)
		 {
			if (number != 3.14) throw new Exception("number converted incorrectly");
		 }
         """
	And the specflow configuration is
         """
		<specFlow>
			<bindingCulture name="de-DE" />
		</specFlow>
         """
	When I execute the tests
	Then the execution summary should contain
         | Succeeded |
         | 1         |
	