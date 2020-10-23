# Local Setup

## Clone the code

Clone the repository with submodules

> git clone --recurse-submodules https://github.com/techtalk/SpecFlow.git

You need to clone the repository with submodules, because the code for the SpecFlow.TestProjectGenerator is located in another repository (https://github.com/techtalk/SpecFlow.TestProjectGenerator). The reason is, that this code is shared with other projects

## set environment variables

### MSBUILDDISABLENODEREUSE

You have to set MSBUILDDISABLENODEREUSE to 1.
Reason for this is, that SpecFlow has an MSBuild Task that is used in the TechTalk.SpecFlow.Specs project. Because of the using of the task and MSBuild reuses processes, the file is loaded by MSBuild and will then lock the file and break the next build.

This environment variable controls the behaviour if MSBuild reuses processes. Setting to 1 disables this behaviour.

See https://github.com/Microsoft/msbuild/wiki/MSBuild-Tips-&-Tricks for more info about it.
