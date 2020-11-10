Feature: Comments

@property:text=Lorems
Scenario: comment wrapping
Given a comment with text <text>
And the screen height of 400px
When the comment box is shown 
Then the the comment size should not exceed 2 lines
