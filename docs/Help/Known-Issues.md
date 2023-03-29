# Known Issue

## Using the "Rule" Gherkin keyword breaks syntax highlighting in Visual Studio 2019 or earlier

The Visual Studio 2017/2019 extension does not yet support the "Rule" Gherkin keyword. Therefore, using this keyword will stop syntax highlighting from working in Visual Studio. Syntax highlighting for the "Rule" keyword will be added in a future release.

To be able to work with rules in Visual Studio, you have to use Visual Studio 2022 or use the "Deveroom" extension for Visual Studio 2017 and 2019. 

## Generating step definitions provides deprecated code

The "[Generate Step Definitions](../visualstudio/Generating-Skeleton-Code)" command generates obsolete code in Visual Studio. E.g.:

![Obsolete code](/_static/images/ObsoleteGeneratedCode.png)

A warning is shown in Visual Studio:

![Obsolete code](/_static/images/Warning.png)

The generation of obsolete code will be removed in a future release.
