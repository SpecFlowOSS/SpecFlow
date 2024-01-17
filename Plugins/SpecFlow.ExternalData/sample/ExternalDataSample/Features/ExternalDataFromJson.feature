Feature: External Data from Json file

@DataSource:products.json
Scenario: The basket price is calculated correctly
	The scenario will be treated as a scenario outline with the examples from the json file.
	The first object array is used by default from the json file.
	Given the price of <product> is €<price>
	And the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be €<price>

@DataSource:products.json @DataSet:other_products
Scenario: The basket price is calculated correctly for other products
	The scenario will be treated as a scenario outline with the examples from the json file.
	The "other_products" object array is used from the json file.
	Given the price of <product> is €<price>
	And the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be €<price>


@DataSource:products-nested-dataset.json
Scenario: The basket price is calculated correctly for products in nested products json
	The scenario will be treated as a scenario outline with the examples from the json file.
	The first object array is used by default from the json file.
	Given the price of <product> is €<total price>
	And the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be €<total price>

@DataSource:products-nested-dataset.json @DataSet:products.varieties
Scenario: The basket price is calculated correctly for products.varieties in nested products json
	The scenario will be treated as a scenario outline with the examples from the json file.
	The products.varieties is used from the json file.
	Given the price of <product> is €<price>
	And the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be €<price>
