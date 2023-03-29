Welcome to SpecFlow's documentation!
====================================

SpecFlow is a test automation solution for .NET built upon the BDD paradigm. Use SpecFlow to define, manage and automatically execute human-readable acceptance tests in .NET projects (Full Framework and .NET Core).

SpecFlow tests are written using `Gherkin <https://cucumber.io/docs/gherkin/>`_, which allows you to write test cases using natural languages. SpecFlow uses the official Gherkin parser, which supports over 70 languages. These tests are then tied to your application code using so-called :doc:`bindings <Bindings/Bindings>`, allowing you to execute the tests using the testing framework of your choice. You can also execute your tests using SpecFlow's own dedicated test runner, SpecFlow+ Runner.

===================
SpecFlow components
===================

* SpecFlow (open-source): This is the core of SpecFlow, providing the functions for binding Gherkin feature files. 

* `SpecFlow+ Runner <https://specflow.org/plus/documentation/SpecFlowPlus-Runner/>`_ (closed-source): This is SpecFlow's dedicated test runner, and provides additional features such as `advanced execution options <https://specflow.org/plus/documentation/Execution/>`_ and `execution reports <https://specflow.org/plus/documentation/Reports/>`_ (HTML, XML, JSON). SpecFlow+ Runner is free of charge, and only requires a free `SpecFlow Account <https://specflow.org/2020/introducing-the-specflow-account/>`_.

* `SpecFlow+ LivingDoc <https://specflow.org/plus/documentation/SpecFlowPlus-LivingDoc/>`_ (closed-source): This is a set of tools that renders your Gherkin Feature Files in an easily readable format with syntax highlighting and allows you to quickly share and collaborate on Gherkin Feature Files with stakeholders that are not familiar with developer tools (such as Visual Studio). 

 * `SpecFlow+ LivingDoc Generator <https://specflow.org/blog/introducing-the-specflow-plus-livingdoc-generator/>`_ is available set of plugins and tools for SpecFlow to generate a local or self-hosted documentation out of your Gherkin feature files, which can be easily shared. No SpecFlow account needed.
 * `SpecFlow+ LivingDoc Azure DevOps <https://marketplace.visualstudio.com/items?itemName=techtalk.techtalk-specflow-plus>`_ is an extension for Azure DevOps/TFS. You can view the output directly in Azure DevOps/TFS, meaning that anyone with access to the system can easily review your specifications when needed. SpecFlow+ LivingDoc Azure DevOps is free of charge, and only requires a free `SpecFlow Account <https://specflow.org/2020/introducing-the-specflow-account/>`_.

SpecFlow also includes a `Visual Studio extension <Tools/Visual-Studio-Integration.html>`_ that adds a number of helpful features to Visual Studio (e.g. Intellisense, feature file templates, context menu entries). However, SpecFlow is not tied to Visual Studio; you can use SpecFlow with Mono or VSCode as well.

==================
Let's get started
==================

You can find a number of step- by- step guides to start with SpecFlow `here <https://docs.specflow.org/projects/getting-started/en/latest/>`_. There are guides available for both complete beginners and more advanced users.

.. toctree::
   :maxdepth: 1
   :caption: Getting Started
   :hidden:

   Getting Started Step by Step Guides <https://docs.specflow.org/projects/getting-started/en/latest/>
   
   

.. toctree::
   :maxdepth: 1
   :caption: Guides & Examples
   :hidden:

   ui-automation/Selenium-with-Page-Object-Pattern.md
   Guides/PageObjectModel.md
   Guides/DriverPattern.md
   Guides/multiplebrowser.md
   Guides/externaldata.md
   Getting-Started/Getting-Started-With-An-Example.md
   Examples <https://docs.specflow.org/en/latest/Examples.html>
  

.. toctree::
   :maxdepth: 2
   :caption: Installation
   :hidden:

   Installation/Requirements.md
   Installation/Installation.md 
   Installation/NuGet-Packages.md
   Installation/Configuration.md
   Installation/Project-and-Item-Templates.md
   Installation/Unit-Test-Providers.md
   Installation/Breaking-Changes-with-SpecFlow-4.0.md
   Guides/UpgradeSpecFlow3To4.md
   Installation/Breaking-Changes-with-SpecFlow-3.0.md

.. toctree::
   :maxdepth: 1
   :caption: Gherkin
   :hidden:

   Gherkin/Gherkin-Reference.md
   Gherkin/Feature-Language.md

.. toctree::
   :maxdepth: 1
   :caption: Bindings
   :hidden:

   Bindings/Bindings.md
   Bindings/Step-Definitions.md
   Bindings/Cucumber-Expressions.md
   Bindings/Hooks.md
   Bindings/Asynchronous-Bindings.md
   Bindings/Scoped-Step-Definitions.md
   Bindings/Sharing-Data-between-Bindings.md
   Bindings/Context-Injection.md
   Bindings/ScenarioContext.md
   Bindings/FeatureContext.md
   Bindings/Calling-Steps-from-Step-Definitions.md
   Bindings/Step-Argument-Conversions.md
   Bindings/Use-Bindings-from-External-Assemblies.md   
   Bindings/SpecFlow-Assist-Helpers.md
   Bindings/FSharp-Support.md

.. toctree::
   :maxdepth: 1
   :caption: Execution
   :hidden:

   Execution/Executing-SpecFlow-Scenarios.md
   Execution/Executing-Specific-Scenarios.md
   Execution/Mark-Steps-As-Not-Implemented.md
   Execution/SkippingScenarios.md
   Execution/Test-Results.md
   Execution/Parallel-Execution.md
   Execution/Debugging.md
   outputapi/outputapi.md
   Execution/Color-Output.md

.. toctree::
   :maxdepth: 1
   :caption: Integrations
   :hidden:


   Integrations/SpecFlow+Runner-Integration.md
   Integrations/MsTest.md
   Integrations/NUnit.md
   Integrations/xUnit.md   
   Integrations/Azure-DevOps.md
   Integrations/Teamcity-Integration.md
   Integrations/Browserstack.md   
   Integrations/Autofac.md
   Integrations/Windsor.md

.. toctree::
   :maxdepth: 1
   :caption: IDE-Integration: Visual Studio
   :hidden:

   visualstudio/Visual-Studio-Integration.md
   visualstudio/visual-studio-installation.rst
   visualstudio/settings-options.rst   
   visualstudio/Visual-Studio-Integration-Editing-Features.rst
   visualstudio/Visual-Studio-Integration-Navigation-Features.rst
   visualstudio/Visual-Studio-Test-Explorer-Support.md
   visualstudio/Generating-Skeleton-Code.rst

.. toctree::
   :maxdepth: 1
   :caption: IDE-Integration: Rider
   :hidden:

   Rider/rider-installation.md
   Rider/rider-features.md


.. toctree::
   :maxdepth: 1
   :caption: IDE-Integration: Visual Studio Code
   :hidden:

   vscode/vscode-specflow.md
   vscode/test-execution.md
   vscode/vscode-debug.md


.. toctree::
   :maxdepth: 1
   :caption: Extend SpecFlow
   :hidden:

   Extend/Value-Retriever.md
   Extend/Plugins.md   
   Extend/Available-Plugins.md
   Extend/Available-Containers-&-Registrations.md
   Extend/Decorators.md
   

.. toctree::
   :maxdepth: 1
   :caption: Tools
   :hidden:
   
   Tools/Generate-Tests-From-MsBuild.md
   Tools/Reporting.md

.. toctree::
   :maxdepth: 1
   :caption: Contribute
   :hidden:

   Contribute/Prerequisite.md
   Contribute/LocalSetup.md
   Contribute/Definition-of-Terms.md
   Contribute/Projects.md
   Contribute/SpecialFiles.md
   Contribute/potentialProblems.md
   Contribute/Coding-Style.md

.. toctree::
   :maxdepth: 1
   :caption: Legacy
   :hidden:

   legacy/Plugins-(Legacy).md
   legacy/CodedUI.md
   Guides/UpgradeSpecFlow2To3.md
   Tools/Tools.md

.. toctree::
   :maxdepth: 1
   :caption: Help
   :hidden:

   Help/Troubleshooting.md
   Help/Known-Issues.md
   Help/Troubleshooting-Visual-Studio-Integration.md

.. toctree::
   :maxdepth: 1
   :caption: Miscellaneous
   :hidden:

   Misc/Usage-Analytics.md
