# Special files

## Directory.Build.props

Explanation: https://docs.microsoft.com/en-us/visualstudio/msbuild/customize-your-build#directorybuildprops-and-directorybuildtargets

We set here general MSBuild properties that are for all projects.

Important is the PropertyGroup for the different Framework versions. We control here which part of SpecFlow is compiled for which .NET Framework version.

## TestRunCombinations.cs

In this file it is controlled for which combinations the integration tests should be generated.