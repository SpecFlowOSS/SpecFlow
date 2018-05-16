Feature: Obsolete Steps
    In order to be able to migrate steps to newer versions
    I would like to be able to mark steps as obsolete and decide in what behavior will that result
    As a developer

Background: 
    Given the following binding class
        """
        [Binding]
        public class StepsWithObsoletion
        {
            [Given(@"Stuff is done")]
            [Obsolete]
            public void GivenStuffIsDone()
            {
                var x = 2+3;
            }
        }
        """	
    And a scenario 'Scenario With Obsolete Step' as
         """
         Given Stuff is done        
         """

Scenario Outline: Obsolete step should cause different behaviors based on configuration
    Given obsoleteBehavior configuration value is set to <ObsoleteBehavior>
    When I execute the tests
    Then the execution summary should contain 
         | Total | Succeeded        | Failed        | Pending        | Ignored |
         | 1     | <SucceededCount> | <FailedCount> | <PendingCount> | 0       |

Examples: 
    | ObsoleteBehavior | SucceededCount | FailedCount | PendingCount |
    | None             | 1              | 0           | 0            |
    | Warn             | 1              | 0           | 0            |
    | Pending          | 0              | 0           | 1            |
    | Error            | 0              | 1           | 0            |


Scenario: Obsolete step should have default warning message
    Given obsoleteBehavior configuration value is set to Warn
    When I execute the tests
    Then the execution log should contain text 'warning: The step GivenStuffIsDone is obsolete because it is marked with ObsoleteAttribute but no custom message was provided.'


Scenario: Obsolete step should use Obsolete Attribute message if present
    Given the following binding class
        """
        [Binding]
        public class StepsWithReason
        {
            [Given(@"Some old stuff")]
            [Obsolete("This step is old.")]
            public void SomeOldStuff()
            {
                var x = 5+1;
            }
        }
        """	
    And a scenario 'Scenario With Obsolete Step with Message' as
         """
         Given Some old stuff      
         """
    Given obsoleteBehavior configuration value is set to Warn
    When I execute the tests
    Then the execution log should contain text 'warning: The step SomeOldStuff is obsolete because This step is old.'