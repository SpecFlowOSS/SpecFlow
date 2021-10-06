Generating Skeleton Code
========================

You can automatically create a suitable class with skeleton bindings and
methods in Visual Studio:


.. tabs::

   .. tab:: VS2019
         
         * Open your feature file.
         
         * Right-click in the editor and select **Generate Step Definitions** from the menu.

         * A dialog is displayed with a list of the steps in your feature file. Use the check boxes to determine which steps to generate skeleton code for.
         
         * Enter a name for your class in the **Class name** field.
        
         * Choose your desired `Step Definition Style <https://docs.specflow.org/projects/specflow/en/latest/Bindings/Step-Definitions.html#step-matching-styles-rules>`__, which include formats without regular expressions. Click on **Preview** to preview the output.
        
         * Click on **Generate** to add a new .cs file with your class to your project. This file will contain the skeleton code for your class and the selected steps.

         .. tip::
            If your feature file already contains bound steps, i.e the binding file already exists and you like to add a new one, click on **Copy methods to
            clipboard** to copy the generated skeleton code to the clipboard. You can then paste it to the file of your choosing.

         The most common parameter usage patterns (quotes, apostrophes, numbers) are detected automatically when creating the code and are used by
         SpecFlow to generate methods and regular expressions.For more information on the available options and custom templates, refer to the `Step Definition Style <https://docs.specflow.org/projects/specflow/en/latest/Bindings/Step-Definitions.html#step-matching-styles-rules>`__ page.
      .. figure:: /_static/images/generatedef2019.gif
         :alt: vs2019

         
          
                     

   .. tab:: VS2022

         * Open your feature file.
         
         * Right-click in the editor and select **Define steps...** from the menu.
         
         * Enter a name for your class in the **Class name** field.

         * Click on **create** to generate a new step definition file or use the **Copy methods to clipboard** method to paste the skeleton code in an existing step definition file.
          
      .. figure:: ../_static/images/generatedef2022.gif
         :alt: vs2022
