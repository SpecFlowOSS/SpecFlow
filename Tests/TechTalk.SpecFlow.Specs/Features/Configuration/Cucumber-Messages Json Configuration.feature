Feature: Cucumber-Messages Configuration via specflow.json

	Cucumber- Messages are disabled by default
	The default Cucumber-Messages file is located at cucumbermessages\messages
	
	When sinks are configured, the default file is not created



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
	Then the default Cucumber-Messages file is created

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
	Then the configured Cucumber-Messages file 'custom_cucumber_messages_file.cm' is created


