@focus
Feature: External Data from Excel file

@DataSource:products.xlsx
Scenario: The basket price is calculated correctly
	The scenario will be treated as a scenario outline with the examples from the CSV file.
	The CSV file contains multile fields, including product and price.
	Given the price of <product> is €<price>
	And the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be €<price>
