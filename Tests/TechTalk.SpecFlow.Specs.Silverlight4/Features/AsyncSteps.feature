#TODO: generate async test from this file only
Feature: Asynchronous steps
	In order to test asynchronous processes
	As a developer
	I want to be able to run steps in an asynchronous manner

# By necessity, these steps aren't strictly adhering to being just Given, When or Then
# Don't use these as an indicator of how to write good step descriptions
Scenario: Running an async process and waiting for it to complete using EnqueueConditional
	When I initiate an asynchronous process
	And it has not yet completed
	And I wait for it
	Then it has completed

Scenario: Running an async process and waiting for it to complete using EnqueueDelay
	When I initiate an asynchronous process
	And it has not yet completed
	And I sleep for 3 seconds
	Then it has completed

Scenario: Using EnqueueDelay before executing the step using EnqueueCallback
	When I sleep before doing executing an action
	Then the action did not start until after the delay

Scenario: Steps called from step definitions are executed in the correct order
	When I call a step and pass value '1'
	And I call a step and pass value '2'
	And I call the step again 5 times from the step definition, passing in an increasing value starting with '3'
	And I call a step and pass value '8'
	And I call the step again 4 times from the step definition, passing in an increasing value starting with '9'
	Then the values passed to the steps should all be in order

Scenario: Nested steps are enqueued to run asynchronously
	When I call a step from a step definition it is not exectued until after this step
	But it is executed before this step
