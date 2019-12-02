Feature: Step definitions can be matched based on the method name (without Regex)

Scenario: Parameterless steps 
	Given a scenario 'Simple Scenario' as
         """
			When I do something
         """
    And a 'When' step definition with name 'When_I_do_something'
	When I execute the tests
	Then the binding method 'When_I_do_something' is executed

Scenario Outline: Steps with parameters
	Given a scenario 'Simple Scenario' as
         """
			When <parameter> does something
         """
	And the following step definitions
		 """
			[When]
			public void When_WHO_does_something(string who)
			{{
                global::Log.LogStep(); 
				if (who != "Joe") throw new Exception("invalid parameter: " + who);
			}}
		 """
	When I execute the tests
	Then the binding method 'When_WHO_does_something' is executed
	And the scenario should pass

Examples: 
	| case        | parameter |
	| simple      | Joe       |
	| quoted      | "Joe"     |
	| apostrophed | 'Joe'     |

Scenario: Steps with parameters referred by index
	Given a scenario 'Simple Scenario' as
         """
			When Joe does something with
         """
	And the following step definitions
		 """
			[When]
			public void When_P0_does_P1_with(string who, string what)
			{{
                global::Log.LogStep();  
			}}
		 """
	When I execute the tests
	Then the binding method 'When_P0_does_P1_with' is executed


Scenario: Supports punctuation
	Given a scenario 'Simple Scenario' as
         """
			When Joe, the man does something with:
				| table |
         """
	And the following step definitions
		 """
			[When]
			public void When_WHO_the_man_does_something_with(string who, Table table)
			{{
                global::Log.LogStep(); 
			}}
		 """
	When I execute the tests
	Then the binding method 'When_WHO_the_man_does_something_with' is executed


Scenario: Keyword prefix can be omitted
	Given a scenario 'Simple Scenario' as
         """
			When I do something
         """
    And a 'When' step definition with name 'I_do_something'
	When I execute the tests
	Then the binding method 'I_do_something' is executed


Scenario Outline: Supports all attributes
	Given a scenario 'Simple Scenario' as
         """
			<step> I do something
         """
    And a '<attribute>' step definition with name 'I_do_something'
	When I execute the tests
	Then the binding method 'I_do_something' is executed

Examples: 
	| attribute      | step  |
	| Given          | Given |
	| When           | When  |
	| Then           | Then  |
	| StepDefinition | Given |

Scenario Outline: Pascal case methods
	Given a scenario 'Simple Scenario' as
         """
			When I do something really important
         """
	And the following step definitions
		 """
			[When]
			public void <method>(string howMuch)
			{{
                global::Log.LogStep(); 
			}}
		 """
	When I execute the tests
	Then the binding method '<method>' is executed once

Examples:
	| case                  | method                             |
	| embedded param        | WhenIDoSomethingHOWMUCHImportant   |
	| param with underscore | WhenIDoSomething_HOWMUCH_Important |

@fsharp
Scenario Outline: F# method name can be used as a regex
	Given there is a SpecFlow project
		And there is an external F# class library project 'ExternalSteps'
		And the following step definition in the project 'ExternalSteps'
			"""
			let [<When>] <method> = LocalApp.Log.LogStep @"<method>"
			"""
		And there is a reference between the SpecFlow project and the 'ExternalSteps' project
		And a scenario 'Simple Scenario' as
         """
         When I do something really important
         """
	And the specflow configuration is
        """
		<specFlow>
			<stepAssemblies>
				<stepAssembly assembly="ExternalSteps" />
			</stepAssemblies>
		</specFlow>
        """
	When I execute the tests
	Then all tests should pass

Examples: 
	| case                  | method                                             |
	| simple                | ``I do something really important``()              |
	| basic regex ops       | ``I do something .* important``()                  |
	| parameter             | ``I do something (.*) important``(howMuch: string) |
	| non-regex method name | When_I_do_something_really_important()             |

Scenario Outline: Non-English keywords
	The the localized prefixes are detected if the feature language or the binding culture is set in the config. 
	In any case, the English prefixes and the prefix-less method names will always work.
	Given there is a feature file in the project as
		"""
			Funktionalität: German
			Szenario: Zwei Zahlen hinzufügen
				<keyword> ich Knopf drücke
		"""
    And a 'Given' step definition with name '<method prefix>ich_Knopf_drücke'
    And the feature language is 'de-DE'
	When I execute the tests
	Then the binding method '<method prefix>ich_Knopf_drücke' is executed once

Examples: 
	| case                           | keyword     | method prefix |
	| No prefix                      | Angenommen  |               |
	| English prefix                 | Angenommen  | Given_        |
	| Single word licalized prefix   | Angenommen  | Angenommen_   |
	| Multiple word licalized prefix | Gegeben sei | Gegeben_sei_  |
	| Mixed keyword variants         | Gegeben sei | Angenommen_   |
