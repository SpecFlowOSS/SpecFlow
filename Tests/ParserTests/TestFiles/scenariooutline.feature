Feature: a feature with a scenario outline

Scenario Outline: a simple scenario outline
Given some <templated> precondition
	| code	| rate	| date	 		| error code  |
	| USD	| 1.2	| <date1>	| OK          |
	| EUR	| 1.2	| <date2>	| OK          |
When I do something
Then something <templated> happens

Examples: first examples set
	| templated	| date1	     | date2      |
	| one       | 2009/09/14 | 2009/09/14 |
	| two       | 2009/09/15 | 2009/09/15 |

Scenarios: second example set
	| templated	| date1	     | date2      |
	| three     | 2009/09/14 | 2009/09/14 |

#third example set without a name
Scenarios:
	| templated	| date1	     | date2      |
	| four      | 2009/09/14 | 2009/09/14 |

Examples: last example set with non-unique first column
	| templated	| date1	     | date2      |
	| five      | 2009/09/14 | 2009/09/14 |
	| five      | 2009/09/15 | 2009/09/15 |
