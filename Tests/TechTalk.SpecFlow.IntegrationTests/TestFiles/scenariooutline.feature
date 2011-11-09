Feature: Scenario outline

Scenario Outline: Simple Scenario Outline
	Given there is something
	When I do <what>
	Then something should happen
Examples: 
	| what           |
	| something      |
	| somethign else |

Scenario Outline: Scenario Outline with multiple examples block
	Given there is something
	When I do <what>
	Then something should happen

Examples: first set
	| what           |
	| something      |
	| somethign else |

Examples: second set
	| what                |
	| somethign different |

#third example set without a name
Examples: 
	| what                        |
	| somethign totally different |

Scenario Outline: Scenario Outline with table arguments
	Given there is something
		| foo    | bar |
		| <what> | xyz |
	When I do <what>
	Then something should happen
Examples: 
	| what           |
	| something      |
	| somethign else |

Scenario Outline: Scenario Outline with multiline string arguments
	Given there is something
		"""
			long text <what>
		"""
	When I do <what>
	Then something should happen
Examples: 
	| what           |
	| something      |
	| somethign else |
