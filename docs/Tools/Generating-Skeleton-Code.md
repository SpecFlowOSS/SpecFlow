# Generating Skeleton Code

You can automatically create a suitable class with skeleton bindings and methods in Visual Studio. To do so:

1. Open your feature file.
1. Right-click in the editor and select **Generate Step Definitions** from the menu. 
1. A dialog is displayed with a list of the steps in your feature file. Use the check boxes to determine which steps to generate skeleton code for.
1. Enter a name for your class in the **Class name** field.
1. Choose your desired [[step definition style|step definition styles]], which include formats without regular expressions. Click on **Preview** to preview the output.
1. Either  
  * Click on **Generate** to add a new .cs file with your class to your project. This file will contain the skeleton code for your class and the selected steps.  
  * Click on **Copy methods to clipboard** to copy the generated skeleton code to the clipboard. You can then paste it to the file of your choosing. Use this method to extend your bindings if new steps have been added to a feature file that already contains bound steps.

The most common parameter usage patterns (quotes, apostrophes, numbers) are detected automatically when creating the code and are used by SpecFlow to generate methods and regular expressions. 

For more information on the available options and custom templates, refer to the [[Step Definition Styles]] page.