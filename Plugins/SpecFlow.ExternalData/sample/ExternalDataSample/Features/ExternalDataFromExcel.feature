Feature: External Data from Excel file

@DataSource:products.xlsx
Scenario: The basket price is calculated correctly
	The scenario will be treated as a scenario outline with the examples from the Excel file.
	The first sheet is used by default from the Excel file.
	Given the price of <product> is €<price>
	And the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be €<price>


@DataSource:products.xlsx @DataSet:other_products
Scenario: The basket price is calculated correctly for other products
	The scenario will be treated as a scenario outline with the examples from the Excel file.
	The "other_products" sheet is used from the Excel file.
	Given the price of <product> is €<price>
	And the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be €<price>
