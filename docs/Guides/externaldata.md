# External Data Plugin

You can easily apply standardized test cases across a wide range of features to significantly reduce
redundant data for large test suites. By reusing execution flows, you can also speed up exploratory and approval testing
for ranges of examples. SpecFlow makes all of this possible by introducing support for loading external data into
scenarios easily.

The [SpecFlow ExternalData plugin](https://www.nuget.org/packages/SpecFlow.ExternalData/) lets teams separate test data from test scenarios, and reuse examples across a large set of scenarios. This is particularly helpful when a common set of examples needs to be consistently verified in different scenarios.

Simply download the [NuGet package](https://www.nuget.org/packages/SpecFlow.ExternalData/) and add it to your specflow projects to use it.

## Supported Data Sources

- CSV files (format 'CSV', extension .csv)

***> Note:** Standard RFC 4180 CSV format is supported with a header line (plugin uses [CsvHelper](https://github.com/JoshClose/CsvHelper) to parse the files).*

- Excel files (format Excel, extensions .xlsx, .xls, .xlsb)

***> Note:** Both XLSX and XLS is supported (plugin uses [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader) to parse the files).*

## Tags

The following tags can be used to specify the external source:

- `@DataSource:path-to-file` - This tag is the main tag that you can add to a scenario or a scenario outline to specify the data source you wish to use.

***> Important:** The path is a relative path to the folder of the **feature files**.*

- `@DisableDataSource` - The `@DataSource` tag can be added to the feature node, turning all scenarios in the file to scenario outlines. This method is useful when the entire feature file uses the same data source. Use the `@DisableDataSource` If you want a select few scenarios in the feature file to **not** use the data source tagged at feature node level.


- `@DataFormat:format` - This tag only needs to be used if the format cannot be identified from the file extension.

- `@DataSet:data-set-name` - This tag is applicable to *Excel files only*. It is used to select the worksheet of the Excel file you wish to use. By **default**, the first worksheet in an Excel file is targeted.

- `@DataField:name-in-feature-file=name-in-source-file` - This tag can be used to "rename" columns of the external data source.

General notes on tags:

- Tags can be added on feature, scenario, scenario outline or scenario outline examples.

- Tags can inherit from the feature node, but you can override them with another tag or disable them by using the `@DisableDataSource` tag on the scenario level.

- As tags cannot contain spaces, generally the underscore (_) character can be used to represent a space. It is currently not supported to access a file that contains spaces in the file name or in the relative path.

## Examples

### CSV files

The below examples all use the same ***products.csv*** file. The file contains three products and their corresponding prices:

![product csv](/_static/images/productscsv.png)

- This scenario will be treated as a scenario outline with the products from the CSV file replacing the **<product<product>>** parameter in the given statement:

````Gherkin

@DataSource:products.csv
Scenario: Valid product prices are calculated
	Given the customer has put 1 piece of <product> in the basket
	When the basket price is calculated
	Then the basket price should be greater than zero

````

- This scenario will be treated as a scenario outline similar to the above example but uses both **<product<product>>** and **<price<price>>** from the CSV file:

````Gherkin

@DataSource:products.csv
Scenario: The basket price is calculated correctly
	Given the price of <product> is €<price>
	And the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be €<price>

````

- This scenario shows how you can extend the product list using the example table with the ones from the CSV file. A total of 4 products will be added here, 3 from the CSV file plus "Cheesecake" from the example table:

````Gherkin

@DataSource:products.csv
Scenario Outline: Valid product prices are calculated (Outline)	
	Given the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be greater than zero
Examples: 
	| product    |
	| Cheesecake |

````

You may also add the `@DataSource` above the example table if you wish to:

````Gherkin

Scenario Outline: Valid product prices are calculated (Outline, example annotation)
	Given the customer has put 1 pcs of <product> to the basket
	When the basket price is calculated
	Then the basket price should be greater than zero
@DataSource:products.csv
Examples: 
	| product    |
	| Cheesecake |

````

- In this scenario the parameters names do not match the column names in the CSV file but we can address that by using the `@DataField:product-name=product` and `@DataField:price-in-EUR=price` tags:

````Gherkin

@DataSource:products.csv @DataField:product-name=product @DataField:price-in-EUR=price
Scenario: The basket price is calculated correctly (renamed fields)	
	Given the price of <product-name> is €<price-in-EUR>
	And the customer has put 1 piece of <product-name> in the basket
	When the basket price is calculated
	Then the basket price should be €<price-in-EUR>

````

- This scenario is similar to the above scenario with the renaming of the parameters, but the difference is the use of space in the parameter name. Spaces are **not** supported and must be replaced with underscore (_):

````Gherkin

@DataSource:products.csv @DataField:product_name=product @DataField:price-in-EUR=price
Scenario: The basket price is calculated correctly
	
	Given the customer has put 1 piece of <product name> in the basket
	When the basket price is calculated
	Then the basket price should be greater than zero
Examples: 
	| product name |
	| Cheesecake   |

````

### Excel files

You can use Excel files the same way as you do with CSV files with some minor differences:

- Only simple worksheets are supported, where the **header is in the first row** and the data comes right below that. Excel files that contain tables, graphics, etc. are not supported.

- Excel files with multiple worksheets are supported, you can use the `@DataSet:sheet-name` to select the worksheets you wish to target. The plugin uses the **first** worksheet by **default**.

- Use underscores in the `@DataSet` tag instead of spaces if the worksheet name contains spaces.

The below example shows an Excel file with multiple worksheets and we wish to target the last worksheet labelled *"other products"*. We do this by using the `@DataSet:other_products` tag. Note the use of (_) instead of space:

![product excel](/_static/images/excel.png)

````Gherkin

@DataSource:products.xlsx @DataSet:other_products
Scenario: The basket price is calculated correctly for other products
	Given the price of <product> is €<price>
	And the customer has put 1 piece of <product> in the basket
	When the basket price is calculated
	Then the basket price should be €<price>

````

## Language Settings

The decimal and date values read from an Excel file will be exported using the language of the feature file (specified using the `#language` setting in the feature file or in the SpecFlow configuration file). This setting affects for example the decimal operator as in some countries comma (,) is used as decimal separator instead of dot (.). To specify not only the language but also the country use the `#language: language-country` tag, e.g. *#language: de-AT* for *Deutsch-Austria*.

Example: Hungarian uses comma (,) as decimal separator instead of dot (.), so SpecFlow will expect the prices in format 1,23:

This sample shows that the language settings are applied for the data that is being
read by the external data plugin.

````Gherkin

#language: hu-HU
Jellemző: External Data from Excel file (Hungarian)

@DataSource:products.xlsx
Forgatókönyv: The basket price is calculated correctly
	Amennyiben the price of <product> is €<price>
	És the customer has put 1 pcs of <product> to the basket
	Amikor the basket price is calculated
	Akkor the basket price should be €<price>

````
