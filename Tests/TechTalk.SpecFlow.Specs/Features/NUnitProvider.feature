Feature: NUnit unit test provider

Scenario Outline: Should be able to execute scenarios with basic results
    Given there is a SpecFlow project
    And the project is configured to use the NUnit provider
    And a scenario 'Simple Scenario' as
        """
        When I do something
        """
    And all steps are bound and <step definition status>
    When I execute the tests with NUnit
    Then the execution summary should contain
        | Total | <result> |
        | 1     | 1        |

Examples: 
    | result    | step definition status |
    | Succeeded | pass                   |
    | Failed    | fail                   |

Scenario Outline: Should handle scenario outlines
    Given there is a SpecFlow project
    And the project is configured to use the NUnit provider
    And row testing is <row test>
    Given there is a feature file in the project as
        """
            Feature: Simple Feature
            Scenario Outline: Simple Scenario Outline
                Given there is something
                When I do <what>
                Then something should happen
            Examples: 
                | what           |
                | something      |
                | something else |
        """
    And all steps are bound and pass
    When I execute the tests with NUnit
    Then the execution summary should contain
        | Succeeded |
        | 2         |

Examples: 
    | case           | row test |
    | Normal testing | disabled |
    | Row testing    | enabled  |

@config
Scenario: Should be able to specify NUnit provider in the configuration
    Given there is a SpecFlow project
    And a scenario 'Simple Scenario' as
        """
        When I do something
        """
    And all steps are bound and pass
    And the specflow configuration is
        """
        <specFlow>
            <unitTestProvider name="NUnit"/>
        </specFlow>
        """
    When I execute the tests with NUnit
    Then the execution summary should contain
        | Total | 
        | 1     | 

Scenario: Should be able to access TestContext using injection and BeforeScenario hook
    Given there is a SpecFlow project
    And the project is configured to use the NUnit provider
    And the following binding class
        """
        using NUnit.Framework;
        [Binding]
        public class StepsWithTestContextAndBeforeScenario
        {
            private NUnit.Framework.TestContext _testContext;
            public StepsWithTestContextAndBeforeScenario(TestContext testContext)
            {
                _testContext = testContext;
                Assert.IsNotNull(_testContext);
            }

            [BeforeScenario()]
            public void BeforeScenario()
            {
                Assert.IsNotNull(_testContext);
            }

            [When(@"I do something")]
            public void WhenIDoSomething()
            {
                Assert.IsNotNull(_testContext);
            }
        }
        """
    And a scenario 'Simple Scenario' as
        """
        When I do something         
        """
    When I execute the tests with NUnit
    Then the execution summary should contain
        | Succeeded |
        | 1         |