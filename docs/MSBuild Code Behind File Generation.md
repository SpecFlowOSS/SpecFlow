# MSBuild Code Behind File Generation

Since SpecFlow 1.9 it is possible to generate the code-behind files of feature files (*.feature.cs) at compile time.  
This is done via a MSBuild Task and can easily be enabled.

### Pros

- Feature files and code-behind files are always in sync
- you don't have to checkin the feature.cs files into your source control system
- it is working without Visual Studio
- it is working for .NET Full Framework and .NET Core

### Cons

- When adding a new file, the CustomTool has to be removed every time
- Realtime test discovery does only find new tests after a build of the project

## Best practises

### Code-Behind files are in same folder as the feature file

In the past, we made the recommendation that you can move your generated code-behind files into a different folder.  
We don't recommend this anymore.  

The reasons are that you will get problems with up-to-date checks in MSBuild.

Additionally, Microsoft fixed a bug in VS, that the navigation from Test Explorer to the feature file works again (<https://developercommunity.visualstudio.com/content/problem/267390/text-explorer-specflow-tests-goes-to-the-feature-c.html>). For that, they need the code-behind files and they have to find it. Having the generated file in a complete separate folder will break this again.

## How to enable it

### SpecFlow 2.3.* & classic project system

1. add NuGet package `SpecFlow.Tools.MsBuild.Generation` with the same version as SpecFlow to your project
2. add following MSBuild snippet at the end of your project file but within the `<Project>` tag:

    ``` xml
    <Target Name="AfterUpdateFeatureFilesInProject">
        <ItemGroup>
        <Compile Include="**\*.feature.cs" Exclude="@(Compile)" />
        </ItemGroup>
    </Target>
    ```

3. remove all `SpecFlowSingleFileGenerator` custom tools from your feature files.

    ![](https://www.specflow.org/screenshots/CustomTool.png)

#### Known bugs

- sometimes Visual Studio doesn't recognize a changed feature file. To get the code-behind file generated, you have to do a rebuild of the project

### SpecFlow 2.4.* & classic project system

__Please use at least SpecFlow 2.4.1, because there the issue from 2.3.* is fixed.__

1. add NuGet package `SpecFlow.Tools.MsBuild.Generation` with the same version as SpecFlow to your project
2. add following MSBuild snippet at the end of your project file but within the `<Project>` tag:

    ``` xml
    <Target Name="AfterUpdateFeatureFilesInProject">
        <ItemGroup>
        <Compile Include="**\*.feature.cs" Exclude="@(Compile)" />
        </ItemGroup>
    </Target>
    ```
3. remove all `SpecFlowSingleFileGenerator` custom tools from your feature files.

    ![](https://www.specflow.org/screenshots/CustomTool.png)

### SpecFlow 3.0.* & classic project system

1. add NuGet package `SpecFlow.Tools.MsBuild.Generation` with the same version as SpecFlow to your project
2. add following MSBuild snippet at the end of your project file but within the `<Project>` tag:

    ``` xml
    <Target Name="AfterUpdateFeatureFilesInProject">
        <ItemGroup>
        <Compile Include="**\*.feature.cs" Exclude="@(Compile)" />
        </ItemGroup>
    </Target>
    ```
3. remove all `SpecFlowSingleFileGenerator` custom tools from your feature files.

    ![](https://www.specflow.org/screenshots/CustomTool.png)

### SpecFlow 2.4.* & sdk- style project system

__Please use at least SpecFlow 2.4.1, because there the issue from 2.3.* is fixed.__

1. add NuGet package `SpecFlow.Tools.MsBuild.Generation` with the same version as SpecFlow to your project
2. remove all `SpecFlowSingleFileGenerator` custom tools from your feature files.

    ![](https://www.specflow.org/screenshots/CustomTool.png)

### SpecFlow 3.0.* & sdk- style project system

1. add NuGet package `SpecFlow.Tools.MsBuild.Generation` with the same version as SpecFlow to your project
2. remove all `SpecFlowSingleFileGenerator` custom tools from your feature files.

    ![](https://www.specflow.org/screenshots/CustomTool.png)

## Common issues

### After upgrade of the NuGet packages, the code- behind files are not generated at compile time

If you are using the classic project system, it can happen, that the previous added MSBuild target is not at the end now. This is because NuGet doesn't care about manual added stuff and places the MSBuild imports at the end. But the definition of the `AfterUpdateFeatureFilesInProject` target has to be after the imports, because otherwise it will be overwritten with an empty definition and so your code- behind files aren't compiled into the assembly.

### Linked files are not included

If you link feature files into a project, no code- behind file is generated for them.  
See GitHub Issue: <https://github.com/techtalk/SpecFlow/issues/1295>

## More infos about MSBuild:

- Microsoft Documentation: <https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild>
- Short introduction: <https://willich.io/2018/02/04/blog-post-to-my-talk-msbuild-101-was-passiert-wenn-ich-auf-build-klicke/>
