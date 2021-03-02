# Special files

## Directory.Build.props

Explanation can be found [here](https://docs.microsoft.com/en-us/visualstudio/msbuild/customize-your-build#directorybuildprops-and-directorybuildtargets)

In this file we set the general MSBuild properties for all projects.

Important to note is the PropertyGroup for different Framework versions. Here, we control which part of SpecFlow is compiled for which .NET Framework version.

## TestRunCombinations.cs

 This file controls in which combinations the integration tests should be generated.
