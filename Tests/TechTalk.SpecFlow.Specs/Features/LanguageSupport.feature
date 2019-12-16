Feature: .NET Language Code-Behind Generation Support


Scenario Outline: A project with a single scenario should compile successfully in all supported languages and build systems
    Given I have a '<Language>' test project
    And there is a feature file in the project as
        """
        Feature: Simple Feature
        Scenario: Simple Scenario
            When I do something
        """
    And all steps are bound and pass

    When I compile the solution using '<Build Command>'
    And I execute the tests

    Then the execution summary should contain
        | Total | Succeeded |
        | 1     | 1         |

@WindowsOnly
Examples:
    | Description         | Language | Build Command |
    | C# with MSBuild     | C#       | MSBuild       |
    | VB.NET with MSBuild | VB.NET   | MSBuild       |

Examples:
    | Description              | Language | Build Command |
    | C# with dotnet build     | C#       | dotnet build  |
    | VB.NET with dotnet build | VB.NET   | dotnet build  |

Examples:
    | Description               | Language | Build Command  |
    | C# with dotnet msbuild    | C#       | dotnet msbuild |
    | VB.NET with dotnet msuild | VB.NET   | dotnet msbuild |
