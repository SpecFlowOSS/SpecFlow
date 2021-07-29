#language: hu-HU
Jellemző: External Data from Excel file (Hungarian)

Hungarian uses comma (,) as decimal separator instead of dot (.), so SpecFlow
will expect the prices in format like 1,23. 

This sample shows that the culture settings are applied for the data that is being 
read by the extenal data plugin.

@DataSource:products.xlsx
Forgatókönyv: The basket price is calculated correctly
	Amennyiben the price of <product> is €<price>
	És the customer has put 1 pcs of <product> to the basket
	Amikor the basket price is calculated
	Akkor the basket price should be €<price>
