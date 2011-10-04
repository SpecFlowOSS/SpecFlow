Feature: Tagged examples

Scenario Outline: a simple scenario outline
#comment here
Given some <templated> precondition

@tag_on_examples
Examples: examples set
	| templated	| date1	     | date2      |
	| one       | 2009/09/14 | 2009/09/14 |
	| two       | 2009/09/15 | 2009/09/15 |

@another_tag
#comment
Scenario: another scenario