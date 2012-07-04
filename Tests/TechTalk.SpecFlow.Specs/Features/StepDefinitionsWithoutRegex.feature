Feature: Step definitions can be matched based on the method name (without Regex)

Scenario: Parameterless steps 
	Given a scenario 'Simple Scenario' as
         """
			When I do something
         """
	And the following step definitions
		 """
			[When]
			public void When_I_do_something()
			{}
		 """
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
			{
				if (who != "Joe") throw new Exception("invalid parameter: " + who);
			}
		 """
	When I execute the tests
	Then the binding method 'When_WHO_does_something' is executed

Examples: 
	| case        | parameter |
	| simple      | Joe       |
	| quoted      | "Joe"     |
	| apostrophed | 'Joe'     |

Scenario: Steps with parameters referred by index
	Given a scenario 'Simple Scenario' as
         """
			When Joe does something with:
         """
	And the following step definitions
		 """
			[When]
			public void When_P0_does_P1_with(string who, string what)
			{
			}
		 """
	When I execute the tests
	Then the binding method 'When_P0_does_P1_with' is executed


Scenario: Steps with multiple parameters and punctuation
	Given a scenario 'Simple Scenario' as
         """
			When Joe does - something with:
				| table |
         """
	And the following step definitions
		 """
			[When]
			public void When_WHO_does_WHAT_with(string who, string what, Table table)
			{
				if (what != "something") throw new Exception("invalid parameter: " + what);
			}
		 """
	When I execute the tests
	Then the binding method 'When_WHO_does_WHAT_with' is executed


Scenario: Keyword prefix can be omitted
	Given a scenario 'Simple Scenario' as
         """
			When I do something
         """
	And the following step definitions
		 """
			[When] public void I_do_something()
			{}
		 """
	When I execute the tests
	Then the binding method 'I_do_something' is executed


Scenario Outline: Supports all attributes
	Given a scenario 'Simple Scenario' as
         """
			<step> I do something
         """
	And the following step definitions
		 """
			[<attribute>] public void I_do_something()
			{}
		 """
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
			{}
		 """
	When I execute the tests
	Then the binding method '<method>' is executed

Examples:
	| case                  | method                             |
	| embedded param        | WhenIDoSomethingHOWMUCHImportant   |
	| param with underscore | WhenIDoSomething_HOWMUCH_Important |
	| mixed underscores     | WhenI_Do_SomethingHOWMUCHImportant |

Scenario: Underscore in parameter name
	Given a scenario 'Simple Scenario' as
         """
			When Joe does something
         """
	And the following step definitions
		 """
			[When]
			public void When_W_H_O_does_something(string w_h_o)
			{
			}
		 """
	When I execute the tests
	Then the binding method 'When_W_H_O_does_something' is executed

@fsharp
Scenario Outline: F# method name can be used as a regex
	Given there is an external F# class library project 'ExternalSteps_FSharp'
	And the following step definition in the external library
        """
		let [<When>] <method> = ()
        """
	And there is a SpecFlow project with a reference to the external library
	And a scenario 'Simple Scenario' as
         """
         When I do something really important
         """
	And the specflow configuration is
        """
		<specFlow>
			<stepAssemblies>
				<stepAssembly assembly="ExternalSteps_FSharp" />
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
	And the following step definitions
		 """
			[Given]
			public void <method prefix>ich_Knopf_drücke()
			{}
		 """
	And the specflow configuration is
         """
		<specFlow>
			<!-- the localized prefixes are detected if the 
				 feature language or the binding culture is set in the config -->
			<language feature="de-DE" /> 
			<!--<bindingCulture name="de-DE" />-->
		</specFlow>
         """
	When I execute the tests
	Then the binding method '<method prefix>ich_Knopf_drücke' is executed

Examples: 
	| case                           | keyword     | method prefix |
	| No prefix                      | Angenommen  |               |
	| English prefix                 | Angenommen  | Given_        |
	| Single word licalized prefix   | Angenommen  | Angenommen_   |
	| Multiple word licalized prefix | Gegeben sei | Gegeben_sei_  |
	| Mixed keyword variants         | Gegeben sei | Angenommen_   |
