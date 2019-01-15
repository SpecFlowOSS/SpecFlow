# MSBuild Code Behind File Generation

Since SpecFlow 1.9, your can  generate the code-behind files for feature files (*.feature.cs) at compile time.  
To do so, you need to use a MSBuild Task.

### Pros

- Feature files and code-behind files are always in sync
- No need to check the feature.cs files into your source control system
- Works without Visual Studio
- Works for both .NET Full Framework and .NET Core

### Cons

- When adding a new file, the CustomTool entered in the feature file's properties has to be removed each time
- Realtime test discovery will only find new tests after the project has been built

## Best practises

### Store code-behind files in same folder as the feature file

In the past, werecommended moving the generated code-behind files to a different folder from your feature files.  
We no longer recommend this approach, as you will otherwise experience problems with up-to-date checks in MSBuild.

Additionally, Microsoft has since fixed a bug in VS, meaning that navigating from the Test Explorer to the feature file works again (<https://developercommunity.visualstudio.com/content/problem/267390/text-explorer-specflow-tests-goes-to-the-feature-c.html>). For this to work, the code-behind files need to be located, and having the generated files in a separate folder will break this feature again.


### Known bugs

- Prior to SpecFlow 2.4.1, Visual Studio sometimes does not recognize that a feature file has changed. To generate the code-behind file, you need to rebuild your project. We recommend using SpecFlow 2.4.1 or higher.

## How to enable it

### Classic Project System

1. Add the NuGet package `SpecFlow.Tools.MsBuild.Generation` with the same version as SpecFlow to your project
2. Add the following MSBuild snippet to the end of your project file, but still within the `<Project>` tag:

    ``` xml
    <Target Name="AfterUpdateFeatureFilesInProject">
        <ItemGroup>
        <Compile Include="**\*.feature.cs" Exclude="@(Compile)" />
        </ItemGroup>
    </Target>
    ```

3. Remove all `SpecFlowSingleFileGenerator` custom tool entries from your feature files.

    ![](https://www.specflow.org/screenshots/CustomTool.png)


### SDK style project system

__Please use at least SpecFlow 2.4.1, as this version fixes the above issue in 2.3.*.__

1. Add the NuGet package `SpecFlow.Tools.MsBuild.Generation` with the same version as SpecFlow to your project
1. Remove all `SpecFlowSingleFileGenerator` custom tool entries from your feature files.

    ![](https://www.specflow.org/screenshots/CustomTool.png)

## Common issues

### After upgrading the NuGet packages, the code-behind files are not generated at compile time

If you are using the classic project system, the previous MSBuild targetmay no longer be at the end <!-- OF WHAT? the project file? --> now. NuGetignored manually added entries and places the MSBuild imports at the end <!-- OF WHAT? the project file? -->. However, the `AfterUpdateFeatureFilesInProject` target has needs to be defined after the imports, because otherwise it will be overwritten with an empty definition. If this happens, your code-behind files are not compiled as part of the assembly.

### Linked files are not included

If you link feature files into a project, no code-behind file is generated for them.  
See GitHub Issue: <https://github.com/techtalk/SpecFlow/issues/1295>

## More infos about MSBuild:

- Microsoft Documentation: <https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild>
- Short introduction: <https://willich.io/2018/02/04/blog-post-to-my-talk-msbuild-101-was-passiert-wenn-ich-auf-build-klicke/>
