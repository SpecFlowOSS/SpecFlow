Extension Settings/Options
==========================

The extension settings can be configured as per below:


.. tabs::

   .. tab:: VS2019
         
         Navigate to **Tools \ | Options \ | SpecFlow \ | General\  |** to access the extension settings.
      .. figure:: /_static/images/vs2019settings.png
         :alt: vs2019
                 
                     

   .. tab:: VS2022

         You must edit the `specflow.json <https://specflow.org/wp-content/uploads/specflowconfigs/specflow-config.json>`__ config file to access the extension settings.
         If you don't have the specflow.json file you can add it by right clicking on the SpecFlow project -> Add -> New item... -> Add SpecFlow configuration file.
         
         The configuration file has a JSON `schema <https://specflow.org/wp-content/uploads/specflowconfigs/specflow-config.json>`__ , therefore you will see all available properties as you start typing.
      .. important:: You must build your project for the changes in specflow.json to take effect. 
          
      .. figure:: ../_static/images/vs2022configfile.png
         :alt: vs2022