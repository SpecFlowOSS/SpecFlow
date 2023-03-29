# Contributing to SpecFlow

Contributing can be a rewarding way to teach, improve existing skills, refine the software you work with and build experience. Contributing to open source can also help you grow and learn more in your career or even change careers!

By contributing to SpecFlow, you also have the chance to become a SpecFlow Community Hero! Please check the [SpecFlow Community Hero Program](https://specflow.org/community/community-hero-program/) for more details.

## What do I need to know to help?

We do all of our development [on GitHub](https://github.com/SpecFlowOSS/SpecFlow). If you are not familiar with GitHub or pull requests please check out [this guide](https://guides.github.com/activities/hello-world/) to get started.

Other prerequisites to develop are : 

- .NET 5 SDK

- .NET Core 3.1 SDK
- .NET Core 2.1 SDK
- .NET 4.6.1 SDK
- .NET 4.7.1 SDK

and of course **C# knowledge** if you are looking to contribute by coding.

## Types of contributions 

You can contribute by working on an  [existing bug/issue](https://github.com/SpecFlowOSS/SpecFlow/search?type=Issues) or report a new one, build a new functionality based on [feature requests](https://support.specflow.org/hc/en-us/community/topics/360000519178-Feature-Requests) reported by SpecFlow community or if do not wish to code you can always contribute to [writing documentation](##Building-documentation). 

### Ground rules & expectations

#### Bug reports

If you like to contribute by fixing a bug/issue, please start by [checking if the issue has already been reported](https://github.com/SpecFlowOSS/SpecFlow/search?type=Issues). 

Guidelines for bug reports:

1. **Use the GitHub issue search** — look for [existing issues](https://github.com/SpecFlowOSS/SpecFlow/search?type=Issues).

2. **Check if the issue has been fixed** &mdash; try to reproduce it using the
   `master` branch in the repository.

3. **Isolate and report the problem** &mdash; ideally create a reduced test
   case. Fill out the provided template.

We label issues that need help, but may not be of a critical nature or require intensive SpecFlow knowledge, to [Up For Grabs](https://github.com/SpecFlowOSS/SpecFlow/labels/up-for-grabs). This is a list of easier tasks that anybody who wants to get into SpecFlow development can try.

#### Feature requests

Feature requests are welcome. But please take a moment to find out whether your idea fits with the scope and aims of the project. It's up to *you*
to make a strong case to convince the community of the merits of this feature. Please visit our [feature request page](https://support.specflow.org/hc/en-us/community/topics/360000519178-Feature-Requests) to check out the existing requests and vote on the ones already proposed by the community. Since much of the work is done by volunteers, someone who believes in the idea will have to write the code.  Please provide as much detail and context as possible.

#### New Features

If you decide to implement one of the existing feature requests or have one of your own, please create an issue before to discuss what and how you are implementing the new feature. There is a possibility that we might not approve your changes, therefore, it is in the interest of both parties to find this out as early as possible to avoid wasting time.

#### Naming Conventions and Reserved ID - NuGet Packages

Microsoft has introduced [package identity verification](https://github.com/NuGet/Home/wiki/NuGet-Package-Identity-Verification#nuget-package-id-prefix-reservation) for packages on nuget.org. This will allow developers to reserve particular ID prefixes used for identification. This in turn should help users identify which packages have been submitted by the owner of the ID prefix.

We have reserved the **“SpecFlow”** NuGet package prefix, which is used to identify official SpecFlow and SpecFlow+ packages. This will mean that new packages with the SpecFlow prefix can only be submitted by SpecFlow, and will indicate that these packages are official.

We have also requested the **"SpecFlow.Contrib"** prefix be made publicly accessible for developers who want to release their own packages for SpecFlow. If you want to submit your own package for SpecFlow whose name begins with “SpecFlow”, you can use this prefix. This will indicate to users that the package is intended for use with SpecFlow, but is a third-party contribution.

These changes will not affect existing packages using the SpecFlow prefix that have already been submitted to nuget.org. If you are the owner of such a package, you should be able to update the package as usual. You may however want to change the name of your package to reflect the new convention.

In summary, here are the prefixes we have:

SpecFlow.*

SpecRun.*

You can find out more about package IDs on [nuget.org blog](https://blog.nuget.org/20170417/Package-identity-and-trust.html).

## How to contribute

As mentioned before, we do all of our development [on GitHub](https://github.com/SpecFlowOSS/SpecFlow). If you are not familiar with GitHub or pull requests please check out [this guide](https://guides.github.com/activities/hello-world/) to get started. All required information about developing SpecFlow can be found in [our documentation](https://docs.specflow.org/projects/specflow/en/latest/Contribute/Prerequisite.html).

Please adhere to the coding conventions in the project (indentation, accurate comments, etc.) and don't forget to add your own tests and documentation. When working with Git, we recommend the following process.

### Pull requests

in order to craft an excellent pull request:

1. [Fork](https://help.github.com/fork-a-repo/) the project, clone your fork, and configure the remotes.

2. Configure your local setup. Information to do this can be found [here](https://docs.specflow.org/projects/specflow/en/latest/Contribute/LocalSetup.html).

3. If you cloned a while ago, get the latest changes from upstream.

4. Create a new topic branch (off of `master`) to contain your feature, change,
   or fix.  

   **IMPORTANT**: Making changes in `master` is discouraged. You should always  keep your local `master` in sync with upstream `master` and make your
   changes in topic branches.

5. Commit your changes in logical chunks. Keep your commit messages organized, with a short description in the first line and more detailed information on the following lines. Feel free to use Git's [interactive rebase](https://help.github.com/articles/interactive-rebase) feature to tidy up your commits before making them public.

6. Newly added tests should pass and be green, same applies to unit tests:

   ![unittests](https://raw.githubusercontent.com/SpecFlowOSS/SpecFlow/master/docs/_static/images/unittests.png)

7. Push your topic branch up to your fork.

8. [Open a Pull Request ](https://help.github.com/articles/using-pull-requests/) with a clear title and description.

9. If you haven't updated your pull request for a while, you should consider rebasing on master and resolving any conflicts.

Some important notes to keep in mind:

- _Never ever_ merge upstream `master` into your branches. You  should always `git rebase` on `master` to bring your changes up to date when  necessary.
- Do not send code style changes as pull requests like changing the indentation of some particular code snippet or how a function is called.
  Those will not be accepted as they pollute the repository history with non functional changes and are often based on personal preferences.
- By submitting a patch, you agree that your work will be licensed under the license used by the project.
- If you have any large pull request in mind (e.g. Implementing features, refactoring code, etc), **please ask first** otherwise you risk spending
  a lot of time working on something that the project's developers might not want to merge into the project. 										

 ## Building sources

Visual Studio:  

- Open <TechTalk.SpecFlow.sln> with Visual Studio
- Build\Build Solution

CLI: 

- Execute build.ps1 in [PowerShell](https://github.com/powershell/powershell)

![buildps1](https://raw.githubusercontent.com/SpecFlowOSS/SpecFlow/master/docs/_static/images/buildps1.png)

## Running tests

The SpecFlow tests are usually multi-platform tests, that means that the same test can be executed multiple times with the different platforms (e.g. .NET Framework 4.7.1, .NET 5, .NET 6). This also means that normally it is not a good idea to just "run all tests", but select a platform for development (.NET 6 is recommended) and run the tests for that one only locally. 

There are unit and integration tests. The unit tests run fast, but the integration tests take more time to run.

Unit test projects:
* TechTalk.SpecFlow.RuntimeTests
* TechTalk.SpecFlow.GeneratorTests
* TechTalk.SpecFlow.PluginTests
* TechTalk.SpecFlow.TestProjectGenerator.Tests
* SpecFlow.ExternalData.SpecFlowPlugin.UnitTests

Integration test projects:
* TechTalk.SpecFlow.MsBuildNetSdk.IntegrationTests
* TechTalk.SpecFlow.Specs

### Running the SpecFlow SpecFlow tests (Specs)

SpecFlow has over 200 SpecFlow scenarios that describe all major behavior. These tests are executed end-to-end, i.e. they create sample projects and solutions, install the interim versions of SpecFlow to these projects and configure them for the particular behavior specified by the scenarios. Because of this, the execution of a single test takes approx. 10 seconds. 

Although the Specs project itself is not cross-compiled for the different platforms (but always runs in .NET 6), the project is configured in a way that it generates multiple tests for each scenario, for the different platforms and the different unit test framework (MsTest, NUnit, xUnit). 

The generated tests have specific categories for the platform (e.g. `Net60`) and the unit test framework (e.g. `xUnit`), so you can filter for these categories to run only one platform and only one unit test framework once. In the Visual Studio Test Window, you can use a filter expression like `Trait:xUnit Trait:Net60`. 

Hints to speed up tests:
* Always select a unit test framework and a platform category to run tests.
* Add the `%TEMP%\SF` folder to the Microsoft Defender Antivirus exclusion list.
* Add the following processes to the Microsoft Defender Antivirus exclusion list: dotnet.exe, MSBuild.exe, vstest.console.exe, VBCSCompiler.exe, testhost.exe, testhost.x86.exe
* Set the `MSBUILDDISABLENODEREUSE` environment variable to `1`

Other notes:
* The Specs tests use an interim version of SpecFlow that you have just built locally. The version number of this package is calculated automatically by the git commits, so after every commit there will be a new version. You might need to do a rebuild to pick it up if you see failures related to not found SpecFlow package versions.
* Unfortunately Visual Studio likes to lock the SpecFlow assemblies. Sometimes this is just an instance of MsBuild that you can kill with Process Explorer, but sometimes you need to restart Visual Studio unfortunately. 

 
## Building documentation

If you do not wish to contribute by coding you can help us in documentation.

To build local documentation:

- Install Python:

  - https://www.python.org/downloads/windows/

    > Note: Make sure to add python to your PATH env variable
    >
    > ![python](https://raw.githubusercontent.com/SpecFlowOSS/SpecFlow/master/docs/_static/images/python.png)

  

- Install sphinx:

  - pip install sphinx

    

- Install dependencies in the working directory

  - pip install -r docs/requirements.txt (Path to requirements can vary)



- Run (PS or CMD) from the working directory
     - ./make html
   - Result: html pages are generated in the working directory
        - _build/html/index.html

## Where can I go for help?

Please join our dedicated [discord server](https://go.specflow.org/join-contributing-on-discord) to ask questions from the SpecFlow team and also get to work with other contributors.



Thank you for your contributions!
