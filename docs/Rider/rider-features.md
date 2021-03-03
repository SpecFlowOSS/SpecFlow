# Features

The SpecFlow for Rider plugin is bundled with a handful of features, see below for more details.

## Navigation

The SpecFlow for Rider plugin allows you to quickly navigate between a Gherkin Step, its relevant bindings, and vice versa.

There are multiple ways to achieve this:

***>Note** The keyboard shortcuts documented below may vary depending on how they were setup during the Rider installation. This documentation is based off the Visual Studio shortcut schema. More info [here](https://www.jetbrains.com/help/rider/Reference_Keymap_VS.html)*.

1- The quickest way to navigate from a Gherkin step to its relevant binding is to use keyboard shortcuts **(Ctrl + F12)**. Alternatively, you can right click on a Gherkin step and navigate to ***Go to ➡ Implementations*** :

![Rider_nav2](../_static/images/rider_nav2.gif)

2- To navigate from a binding to its corresponding Gherkin step, the keyboard shortcuts **(Shift + F12)** can be used :

![Rider_nav3](../_static/images/rider_nav3.gif)

3- You can also see all Gherkin steps currently using a particular binding by pressing **F12** or by navigating to it ***Go to ➡ Go to Declarations or Usages*** :

![Rider_nav4](../_static/images/rider_nav4.gif)

## Creating steps

The plugin allows you to quickly create a step and also highlights when a step is missing. To do this, click on an unbound step, click on view action list, and then click on *Create step*:

![Rider_createstep](../_static/images/rider_createstep.gif)

## Test results

The SpecFlow plugin also displays the test results for a specific Gherkin step. You can see whether a test has passed or failed and also execute it by pressing the **⏵** button:

![Rider_results](../_static/images/rider_result.png)

## Auto renaming

The plugin also automatically detects any mismatch in step definition names and displays suggestions to match it to its pattern:

![Rider_match](../_static/images/rider_match.gif)

![SpecFlow logo](../_static/images/specflow_logov2.png) Please note we are always working to improve and introduce new features to make the plugin more versatile and easy to use.
