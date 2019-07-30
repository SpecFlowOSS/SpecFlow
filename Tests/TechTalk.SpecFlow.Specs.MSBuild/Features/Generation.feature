Feature: Generation

Scenario: No features
Given a project with no features
When the project is built
Then the project should have been compiled without errors

Scenario: Building a feature
Given a project with these features
| Feature |
| Foo     |
When the project is built
Then the project should have been compiled without errors
And the project output should be a test suite for these features
| Feature |
| Foo     |

Scenario: Building multiple features
Given a project with these features
| Feature |
| Foo     |
| Bar     |
When the project is built
Then the project should have been compiled without errors
And the project output should be a test suite for these features
| Feature |
| Foo     |
| Bar     |

Scenario: Repeat build
Given a project with these features which has been built successfully
| Feature |
| Foo     |
| Bar     |
When the project is built
Then the project should have been compiled without errors
And the project output should be a test suite for these features
| Feature |
| Foo     |
| Bar     |

Scenario: Building after a feature is removed
Given a project with these features which has been built successfully
| Feature |
| Foo     |
| Bar     |
When the "Foo" feature is removed
And the project is built
Then the project should have been compiled without errors
And the project output should be a test suite for these features
| Feature |
| Bar     |