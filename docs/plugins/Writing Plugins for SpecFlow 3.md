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

Depending on the Target Framework of the project where the Runtime plugin package is used, we have to copy a different assembly in the output folder.
Because the `$(TargetFrameworkIdentifider)` property is only availabe in imported `target`- Files, we have to calculate here the path to the assembly.

#### build/SpecFlow.SamplePlugin.props

To copy the plugin assembly into the output folder, we include it in the `None`- ItemGroup and set `CopyToOutputDirectory` to `PreserveNewest`. That way, it is copied to the output directory also if you changed it.

#### Directory.Build.targets

To specific the path to the assemblies which should be put into the NuGet package, we have to set some NuSpec properties.
This is done in the `Directory.Build.targets` so it defined for all projects in subfolders. We add the value of the current configuration to the available properties.

#### SamplePlugin.nuspec

This is the NuSpec- File to provide infos for the NuGet package. To get the files from the build- directory into the NuGet package, we have to specify them in the `file`- list.
The runtime plugin assemblies are also specified here. For that, we are using the additional `$config$` property we added in the `Directory.Build.targets`.

## Generator plugin

## Combined Package  with both plugins