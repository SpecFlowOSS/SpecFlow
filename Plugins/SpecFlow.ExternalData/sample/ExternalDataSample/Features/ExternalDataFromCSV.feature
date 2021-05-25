@focus
Feature: External Data from CSV file

Scenario: Without: Valid Chocolate price is calculated
	Given the customer has put 1 pcs of Chocolate to the basket
	When the basket price is calculated
	Then the basket price should be greater than zero

Scenario: Without: The basket price with Chocolate calculated correctly
	Given the price of Chocolate is €2.5
	Given the customer has put 3 pcs of Chocolate to the basket
	When the basket price is calculated
	Then the basket price should be €7.5

Scenario Outline: Without: Valid product prices are calculated
	Given the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be greater than zero
Examples: 
	| product   |
	| Chocolate |
	| Apple     |
