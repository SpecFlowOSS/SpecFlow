# Visual Studio Test Explorer Support

The Visual Studio integration supports executing SpecFlow scenarios from the Visual Studio Test Explorer. The basic Test Explorer features work with all unit test providers, although you may need to install additional Visual Studio connectors, depending on the unit test framework. Full integration is provided for [[SpecRun|http://www.specrun.com]], meaning you can run and debug your scenarios as first class citizens:

* Right-click in a feature file and select **Run/Debug SpecFlow Scenarios** from the menu to run/debug the scenarios and scenario outlines in the file
* View scenarios listed by title in the Test Explorer
* Group scenarios in the Test Explorer by tag (choose "Traits" grouping) or feature (choose "Class")
* Filter scenarios by various criteria using the search field
* Run/debug selected or all scenarios
* Double-click an entry in the list to switch to the scenario in the feature file
* View test execution results