Feature: Cucumber-Messages Configuration via app.config

  Cucumber-messages can be configured in the SpecFlow section of the app.config file to enable/disable messages
  and configure alternative output sinks
  
Scenario: No configuration creates no output file
  
  Given there is a project with this app.config configuration
    """
    <?xml version="1.0" encoding="utf-8"?>
	<configuration>
	  <configSections>
		<section name="specFlow" type="TechTalk.SpecFlow.Configuration.ConfigurationSectionHandler, TechTalk.SpecFlow" />
	  </configSections>
  
	  <specFlow>
		<cucumber-messages />
	  </specFlow>
	</configuration>
    """
  When the test suite is executed
  Then no Cucumber-Messages file is created

Scenario: Disabled configuration creates no output file
  Given there is a project with this app.config configuration
    """
	<?xml version="1.0" encoding="utf-8"?>
	<configuration>
	  <configSections>
		<section name="specFlow" type="TechTalk.SpecFlow.Configuration.ConfigurationSectionHandler, TechTalk.SpecFlow" />
	  </configSections>
  
	  <specFlow>
		<cucumber-messages enabled="false" />
	  </specFlow>
	</configuration>
    """
  When the test suite is executed
  Then no Cucumber-Messages file is created

Scenario: Enabled configuration with no sinks configured create default output file
  The default Cucumber-Messages file is created at `cucumbermessages\messages` relative to the output directory
  
  Given there is a project with this app.config configuration
    """
    <?xml version="1.0" encoding="utf-8"?>
	<configuration>
	  <configSections>
		<section name="specFlow" type="TechTalk.SpecFlow.Configuration.ConfigurationSectionHandler, TechTalk.SpecFlow" />
	  </configSections>
  
	  <specFlow>
		<cucumber-messages enabled="true" />
	  </specFlow>
	</configuration>
    """
  When the test suite is executed
  Then the Cucumber-Messages file 'cucumbermessages\messages' is created

Scenario: Configured sinks are respected
  Given there is a project with this app.config configuration
    """
	<?xml version="1.0" encoding="utf-8"?>
	<configuration>
	  <configSections>
		<section name="specFlow" type="TechTalk.SpecFlow.Configuration.ConfigurationSectionHandler, TechTalk.SpecFlow" />
	  </configSections>
  
	  <specFlow>
		<cucumber-messages enabled="true" >
			<sinks>
				<sink type="file" path="custom_cucumber_messages_file.cm" />
			</sinks>
		</cucumber-messages>
	  </specFlow>
	</configuration>
    """
  When the test suite is executed
  Then the Cucumber-Messages file 'custom_cucumber_messages_file.cm' is created

