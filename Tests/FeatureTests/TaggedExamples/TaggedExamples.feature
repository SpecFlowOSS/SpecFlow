Feature: Tagging Scenario Outline Examples
	As a developer
	I would like to be able to tag examples of the scenario outline
	So that I can write bindings that behave differently for the variants

@tag1
Scenario Outline: Examples can be tagged
	Given I have a templated step for <variant>
	When I tag the examples with tag <tag>
	Then the execution should be scoped with tag <tag>

Examples:
	| variant                        | tag |
	| variant in non-tagged examples |     |

@examples_tag
Examples:
	| variant                    | tag          |
	| variant in tagged examples | examples_tag |

@buildserver_exclude
Scenario Outline: Examples can be ignored
	Given I have invalid step
	When I tag the examples with @ignore
	Then the execution should be ignored

@ignore
Examples:
	| variant         | 
	| ignored variant | 
