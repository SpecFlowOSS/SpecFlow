Feature: Tracing tests

Scenario: Preserves step keywords in trace
	Given there is a feature file in the project as
		"""
			#language: de-DE
			Funktionalität: German
			Szenario: Zwei Zahlen hinzufügen
				Angenommen ich Knopf 1 drücke
				Gegeben sei ich Knopf 2 drücke
		"""
	And all steps are bound and pass
	When I execute the tests
	Then the execution log should contain text 'Angenommen ich Knopf 1 drücke'
	And the execution log should contain text 'Gegeben sei ich Knopf 2 drücke'
