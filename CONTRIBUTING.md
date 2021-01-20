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
