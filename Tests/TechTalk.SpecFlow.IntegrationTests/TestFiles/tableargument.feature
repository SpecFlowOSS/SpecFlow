Feature: Table argument

Scenario: a scenario with a table argument
  Given there is a table argument
	| code	| rate	| date	 		| error code  |
	| USD	| 1.2	| 2009/09/14	| OK          |
	| EUR	| 1.2	| 2009/09/14	| OK          |
  When there is a table argument
	| code	| rate	| date	 		| error code  |
	| USD	| 1.2	| 2009/09/14	| OK          |
	| EUR	| 1.2	| 2009/09/14	| OK          |
  Then there is a table argument
	| code	| rate	| date	 		| error code  |
	| USD	| 1.2	| 2009/09/14	| OK          |
	| EUR	| 1.2	| 2009/09/14	| OK          |
