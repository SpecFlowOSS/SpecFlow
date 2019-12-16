Feature: Cucumber-Messages Configuration via specflow.json

  Cucumber-messages can be configured in the specflow.json file to enable/disable messages
  and configure alternative output sinks
  
Scenario: No configuration creates no output file
  
  Given there is a project with this specflow.json configuration
    """
    {
      "cucumber-messages":
      {
      }
    }
    """
  When the test suite is executed
  Then no Cucumber-Messages file is created

Scenario: Disabled configuration creates no output file
  Given there is a project with this specflow.json configuration
    """
    {
      "cucumber-messages":
      {
        "enabled": false
      }
    }
    """
  When the test suite is executed
  Then no Cucumber-Messages file is created

Scenario: Enabled configuration with no sinks configured create default output file
  The default Cucumber-Messages file is created at `cucumbermessages\messages` relative to the output directory
  
  Given there is a project with this specflow.json configuration
    """
    {
      "cucumber-messages":
      {
        "enabled": true
      }
    }
    """
  When the test suite is executed
  Then the Cucumber-Messages file 'cucumbermessages/messages' is created

Scenario: Configured sinks are respected
  Given there is a project with this specflow.json configuration
    """
    {
      "cucumber-messages":
      {
        "enabled": true,
        "sinks": [
          { "type":"file", "path": "custom_cucumber_messages_file.cm" }
        ]
      }
    }
    """
  When the test suite is executed
  Then the Cucumber-Messages file 'custom_cucumber_messages_file.cm' is created
