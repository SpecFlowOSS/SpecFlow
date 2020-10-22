Feature: User registration

@property:email=E-mail_addresses.Valid
Scenario Outline: recording user information on successful registration

Given a visitor registering as "Mike Scott" with email <email>
When the registration completes
Then the account system should record <email> related to user "Mike Scott"

Examples: key examples
  | Variant            | email              |
  | simple valid email | simple@example.com |

@property:email=E-mail_addresses.Invalid
Scenario: rejecting invalid emails

Given a visitor registering as "Mike Scott" with email <email>
When the registration completes
Then the account system should not record <email> 
And the error response should be "Invalid Email"