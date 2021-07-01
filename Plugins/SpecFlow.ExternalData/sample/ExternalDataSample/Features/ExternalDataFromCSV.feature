@focus
Feature: External Data from CSV file

Scenario: Without: Valid Chocolate price is calculated
	Given the customer has put 1 pcs of Chocolate to the basket
	When the basket price is calculated
	Then the basket price should be greater than zero

Scenario: Without: The basket price with Chocolate is calculated correctly
	Given the price of Chocolate is €2.5
	And the customer has put 3 pcs of Chocolate to the basket
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

@DataSource:products.csv
Scenario: Valid product prices are calculated
	The scenario will be treated as a scenario outline with the examples from the CSV file.
	Given the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be greater than zero

@DataSource:products.csv
Scenario: The basket price is calculated correctly
	The scenario will be treated as a scenario outline with the examples from the CSV file.
	The CSV file contains multile fields, including product and price.
	Given the price of <product> is €<price>
	And the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be €<price>

@DataSource:products.csv
Scenario Outline: Valid product prices are calculated (Outline)
	The provided product list is extended with the ones from the CSV file.
	Given the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be greater than zero
Examples: 
	| product    |
	| Cheesecake |

Scenario Outline: Valid product prices are calculated (Outline, example annotation)
	The provided product list is extended with the ones from the CSV file.
	Given the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be greater than zero
@DataSource:products.csv
Examples: 
	| product    |
	| Cheesecake |

@DataSource:products.csv @DataField:product-name=product @DataField:price-in-EUR=price
Scenario: The basket price is calculated correctly (renamed fields)
	The scenario will be treated as a scenario outline with the examples from the CSV file.
	The CSV file contains multile fields, including 'product' and 'price', 
	but those are different from the ones we want to use in the scenario ('product name' and 'price-in-EUR').
	Given the price of <product-name> is €<price-in-EUR>
	And the customer has put 1 pcs of <product-name> to the basket
	When the basket price is calculated
	Then the basket price should be €<price-in-EUR>

