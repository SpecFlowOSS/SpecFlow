# SETUP

## Clone the code

Clone the repository with submodules

> git clone --recurse-submodules https://github.com/techtalk/SpecFlow.git

You need to clone the repository with submodules, because the code for the SpecFlow.TestProjectGenerator is located in another repository (https://github.com/techtalk/SpecFlow.TestProjectGenerator). The reason is, that this code is shared with other projects


## Setup development environment

### Visual Studio

You will need the latest version or the latest preview version of Visual Studio to work on SpecFlow.

### Environment variables

#### MSBUILDDISABLENODEREUSE

You have to set MSBUILDDISABLENODEREUSE to 1.
Reason for this is, that SpecFlow has a MSBuild Task that is used in the TechTalk.SpecFlow.Specs project. Because of the using of the task and MSBuild reuses processes, the file is loaded by MSBuild and will then lock the file and break the next build.

This environment variable controls the behaviour if MSBuild reuses processes. Setting to 1 disables this behaviour.

See https://github.com/Microsoft/msbuild/wiki/MSBuild-Tips-&-Tricks for more info about it.


## Definition of Terms

### Runtime
Runtime is then, when scenarios are executed by a test runner

### GeneratorTime
GeneratorTime is then, when the code-behind files are generated. 

### Code-Behind files
For every feature file, SpecFlow generates a code-behind file, which contains the code for the various test frameworks.  
It's generated when the project gets compiled. This is done by the SpecFlow.Tools.MsBuild.Generation MSBuilt task.

# Projects

## TechTalk.SpecFlow
This is the runtime part of SpecFlow.

## TechTalk.SpecFlow.Generator
This is the main part of SpecFlow that is used at GeneratorTime

## TechTalk.SpecFlow.Parser
This contains the parser for Feature- Files. We use Gherkin and added some additional features to the object model and parser.

## TechTalk.SpecFlow.Utils
This project contains some helper classes.

## SpecFlow.Tools.MsBuild.Generation
This project contains the MSBuild task that generates the code-behind files.

## TechTalk.SpecFlow.GeneratorTests
This contains unit tests that are about the generation of the code-behind files

## TechTalk.SpecFlow.RuntimeTests
This contains unit tests that are about the runtime of Scenarios

## TechTalk.SpecFlow.MSTest.SpecFlowPlugin
This is the plugin for MSTest. It contains all specific implementations for MSTest.

## TechTalk.SpecFlow.NUnit.SpecFlowPlugin
This is the plugin for NUnit. It contains all specific implementations for NUnit.

## TechTalk.SpecFlow.xUnit.SpecFlowPlugin
This is the plugin for xUnit. It contains all specific implementations for xUnit.

## NuGetPackages
This project generates the NuGet packages

## TechTalk.SpecFlow.TestProjectGenerator
This project provides an API for generating projects, compile them and run tests.

## TechTalk.SpecFlow.TestProjectGenerator.Tests
Tests for TechTalk.SpecFlow.TestProjectGenerator

## TechTalk.SpecFlow.Specs
This project contains the integration tests for SpecFlow

## TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
This is a generator plugin, that generates the integration tests for various combinations.
They can differ in Framework Version, Testing Framework, Project Format and programming language


# Special files

## Directory.Build.props

Explanation: https://docs.microsoft.com/en-us/visualstudio/msbuild/customize-your-build#directorybuildprops-and-directorybuildtargets

We set here general MSBuild properties that are for all projects.

Important is the PropertyGroup for the different Framework versions. We control here which part of SpecFlow is compiled for which .NET Framework version.

## TestRunCombinations.cs

In this file it is controlled for which combinations the integration tests should be generated.


# Plugins

## GeneratorPlugins

IGeneratorPlugin implmeent

`[assembly: GeneratorPlugin(typeof(RuntimePlugin))]`

## Runtime Plugin

IRuntimePlugin implement

`[assembly: RuntimePlugin(typeof(RuntimePlugin))]`


Interface is in TechTalk.SpecFlow defined



# Directory.build.props

## TFS definitions
