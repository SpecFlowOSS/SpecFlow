Feature: GH2571

Parameters of Scenario Outlines contained in multi-line text that are nested inside of XML are not handled properly - https://github.com/SpecFlowOSS/SpecFlow/issues/2571


Scenario: GH2571

    Given there is a feature file in the project as
         """
         Feature: parameters in DocStrings
         Scenario Outline: nested angle brackets surround parameter names
         Given the the package contains the web.config file of the following content
             \"\"\"
             <?xml version="1.0" encoding="utf-8"?>
		     <configuration>
			    <system.Web>
				    <customErrors mode="<Mode>" />
			    </system.Web>
		     </configuration>
             \"\"\"
	        When the user gets the debug status
	        Then the customErrors element should be <customErrors>

	        Examples: 
	        | Mode          | customErrors |
	        | Off           | <customErrors mode="Off" />         |
	        | RemoteOnly    | <customErrors mode="RemoteOnly" />  |
         """

    And the following binding class
        """
        using FluentAssertions;
        using TechTalk.SpecFlow;

            [Binding]
            public class ScenarioOutlineBug2571StepDefinitions
            {
                private string _docString;
                private string _customErrorString;

                public ScenarioOutlineBug2571StepDefinitions()
                {
                    
                }


                [Given("the the package contains the web.config file of the following content")]
                public void GivenMyXMLIs(string scenarioText)
                {

                    _docString = scenarioText;
                }


                [When("the user gets the debug status")]
                public void WhenIProcessXML()
                {
                    
                    int x = _docString.IndexOf("<custom");
                    string line = _docString.Substring(x);
                    int nl = line.IndexOf("system") - 2;
                    _customErrorString = line.Substring(0, nl - 1).Trim();
                }

                [Then("the customErrors element should be (.*)")]
                public void ThenTheResultShouldBe(string expected)
                {
                   _customErrorString.Should().Be(expected);
                }
            }
        """
    
    When I execute the tests
    Then the execution summary should contain
         | Total | Succeeded | Failed |
         | 2     | 2         | 0      |
