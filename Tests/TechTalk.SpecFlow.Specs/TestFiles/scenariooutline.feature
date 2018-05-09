Feature: Scenario outline

Scenario Outline: Simple Scenario Outline
	Given there is something
	When I do <what>
	Then something should happen
Examples: 
	| what           |
	| something      |
	| something else |

Scenario Outline: Scenario Outline with multiple examples block
	Given there is something
	When I do <what>
	Then something should happen

Examples: first set
	| what           |
	| something      |
	| something else |

Examples: second set
	| what                |
	| something different |

#third example set without a name
Examples: 
	| what                        |
	| something totally different |

Scenario Outline: Scenario Outline with table arguments
	Given there is something
		| foo    | bar |
		| <what> | xyz |
	When I do <what>
	Then something should happen
Examples: 
	| what           |
	| something      |
	| something else |

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
	| something else |
