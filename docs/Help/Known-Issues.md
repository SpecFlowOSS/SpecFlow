# Known Issue

## Using the "Rule" Gherkin keyword breaks syntax highlighting in Visual Studio

The Visual Studio extension does not yet support the "Rule" Gherkin keyword. Therefore, using this keyword will stop syntax highlighting from working in Visual Studio. Syntax highlighting for the "Rule" keyword will be added in a future release.

## Generating step definitions provides deprecated code

The "[Generate Step Definitions](../Tools/Generating-Skeleton-Code)" command generates obsolete code in Visual Studio. E.g.:

![Obsolete code](/_static/images/ObsoleteGeneratedCode.png)

A warning is shown in Visual Studio:

![Obsolete code](/_static/images/Warning.png)

The generation of obsolete code will be removed in a future release.
