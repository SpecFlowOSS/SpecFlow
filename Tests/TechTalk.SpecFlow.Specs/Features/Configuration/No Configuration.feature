Feature: No Configuration


Scenario: No configuration file creates no cucumber-message output file

  Given the project has no specflow.json configuration
  And the project has no app.config configuration

  When the test suite is executed
  Then no Cucumber-Messages file is created