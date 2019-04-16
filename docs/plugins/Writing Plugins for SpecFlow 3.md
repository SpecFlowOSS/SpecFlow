# Writing Plugins for SpecFlow 3

With SpecFlow 3 we changed the way how you configure which plugins are used. They aren't configured anymore in the app.config or specflow.json.

## Runtime plugin

Runtime plugins need to target .NET Framework 4.5 and .NET Standard 2.0.
SpecFlow searches all assemblies in the folder of the test assembly for files that end with `.SpecFlowPlugin.dll`.
It loads them in the order it finds it the folder.

### Example

A complete example for a Runtime plugin can be found here: <https://github.com/techtalk/SpecFlow-Examples/tree/master/Plugins/RuntimeOnlyPlugin>  
It packages a Runtime plugin into a NuGet package.

#### SampleRuntimePlugin.csproj

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/SampleRuntimePlugin.csproj>

The project targets multiple frameworks. So it uses `<TargetFrameworks>` and not `<TargetFramework>`.
We set a different `<AssemblyName>` to add the needed `.SpecFlowPlugin` at the end. You could also name your project already with the `.SpecFlowPlugin` at the end.
`<GeneratePackageOnBuild>` is set to true, so that the NuGet package is generated on build.
We use a NuSpec file (SamplePlugin.nuspec) to provde all information for the NuGet package. This is set at the `<NuspecFile>` property.

For a Runtime plugin you only need a reference to the `SpecFlow`- NuGet package.

#### build/SpecFlow.SamplePlugin.targets

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/build/SpecFlow.SamplePlugin.targets>

Depending on the Target Framework of the project where the Runtime plugin package is used, we have to copy a different assembly in the output folder.
Because the `$(TargetFrameworkIdentifider)` property is only availabe in imported `target`- Files, we have to calculate here the path to the assembly.

#### build/SpecFlow.SamplePlugin.props

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/build/SpecFlow.SamplePlugin.props>

To copy the plugin assembly into the output folder, we include it in the `None`- ItemGroup and set `CopyToOutputDirectory` to `PreserveNewest`. That way, it is copied to the output directory also if you changed it.

#### Directory.Build.targets

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/Directory.Build.targets>

To specific the path to the assemblies which should be put into the NuGet package, we have to set some NuSpec properties.
This is done in the `Directory.Build.targets` so it defined for all projects in subfolders. We add the value of the current configuration to the available properties.

#### SamplePlugin.nuspec

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/SamplePlugin.nuspec>

This is the NuSpec- File to provide infos for the NuGet package. To get the files from the build- directory into the NuGet package, we have to specify them in the `file`- list.
The runtime plugin assemblies are also specified here. For that, we are using the additional `$config$` property we added in the `Directory.Build.targets`.

## Generator plugin

Generator plugins need to target .NET Framework 4.7.1 and .NET Standard 2.0.
The MSBuild task needs to know which generator plugins it should use. For this you have to add your generator plugin to the `SpecFlowGeneratorPlugins`- ItemGroup.
This is passed to the MSBuild task as a parameter and used later to load the plugins.

### Example

A complete example for a Runtime plugin can be found here: <https://github.com/techtalk/SpecFlow-Examples/tree/master/Plugins/GeneratorOnlyPlugin>
It packages a Generator plugin into a NuGet package.

#### SampleGeneratorPlugin.csproj

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/SampleGeneratorPlugin.csproj>

The project targets multiple frameworks. So it uses `<TargetFrameworks>` and not `<TargetFramework>`.
We set a different `<AssemblyName>` to add the needed `.SpecFlowPlugin` at the end. You could also name your project already with the `.SpecFlowPlugin` at the end.
`<GeneratePackageOnBuild>` is set to true, so that the NuGet package is generated on build.
We use a NuSpec file (SamplePlugin.nuspec) to provde all information for the NuGet package. This is set at the `<NuspecFile>` property.

For a Generator plugin you  need a reference to the `SpecFlow.CustomPlugin`- NuGet package.

#### build/SpecFlow.SamplePlugin.targets

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/build/SpecFlow.SamplePlugin.targets>

Depending on the used MSBuild version (.NET Full Framework or .NET Core), we have to add a different assembly in the ItemGroup.
Because the `$(MSBuildRuntimeType)` property is only availabe in imported `target`- Files, we have to calculate here the path to the assembly.

#### build/SpecFlow.SamplePlugin.props

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/build/SpecFlow.SamplePlugin.props>

As we have now a property which contains the path to the assembly, we can add it here to the `SpecFlowGeneratorPlugins`- ItemGroup.

#### Directory.Build.targets

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/Directory.Build.targets>

To specific the path to the assemblies which should be put into the NuGet package, we have to set some NuSpec properties.
This is done in the `Directory.Build.targets` so it defined for all projects in subfolders. We add the value of the current configuration to the available properties.

#### SamplePlugin.nuspec

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/SamplePlugin.nuspec>

This is the NuSpec- File to provide infos for the NuGet package. To get the files from the build- directory into the NuGet package, we have to specify them in the `file`- list.
The generator plugin assemblies are also specified here. For that, we are using the additional `$config$` property we added in the `Directory.Build.targets`.
Here it is important, that they don't get added to the `lib` folder. If they were there, they would be referenced by the project where you add the NuGet package. This is something we don't want.

## Combined Package with both plugins

If you need to adjust Generator and Runtime with a single NuGet package (as we are doing with the `SpecFlow.xUnit`, `SpecFlow.NUnit` and `SpecFlow.xUnit` packages), you need to put the Generator and Runtime plugin in a single NuGet package.

As with the seperate plugins, you need two projects. One for Runtime plugin, one for Generator plugin. As you only want one NuGet package, you have to put the NuSpec- file only in one of the projects.
This is in the Generator project. Reason is, the Generator plugin is build with a higher .NET Framework version (.NET 4.7.1) and so you can make a dependency on the Runtime plugin (which is only .NET 4.5).
It doesn't work in the other direction.

You can simply combine the contents of the `.targets` and `.props` file to a single one.

### Example

A complete example for a NuGet package that contains Runtime and Generator plugin can be found here: <https://github.com/techtalk/SpecFlow-Examples/tree/master/Plugins/CombinedPlugin>

The files do the same as above.