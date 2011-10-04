Feature: Given, When, Then duplications

Scenario: a simple scenario with duplicated keywords
Given some precondition
Given some precondition
When I do something
When I do something
Then something happens
Then something happens

Scenario: a simple scenario with mixed keywords
Given some precondition
Given some precondition
And some precondition
When I do something
And I do something
When I do something
Then something happens
But something happens
And something happens
Then something happens
