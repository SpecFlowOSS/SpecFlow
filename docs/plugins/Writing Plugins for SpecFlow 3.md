# Writing Plugins for SpecFlow 3

With SpecFlow 3, we have changed how you configure which plugins are used. They are no longer configured in your `app.config` (or `specflow.json`).

## Runtime plugin

Runtime plugins need to target .NET Framework 4.5 and .NET Standard 2.0.
SpecFlow searches through all assemblies in the folder of your test assembly for files that end with `.SpecFlowPlugin.dll`.
It loads them in the order it finds plugins in the folder.

### Sample runtime plugin

A complete example of a Runtime plugin can be found here: <https://github.com/techtalk/SpecFlow-Examples/tree/master/Plugins/RuntimeOnlyPlugin>  
It packages a Runtime plugin into a NuGet package.

#### SampleRuntimePlugin.csproj

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/SampleRuntimePlugin.csproj>

The project targets multiple frameworks, so it uses `<TargetFrameworks>` and not `<TargetFramework>`.
We set a different `<AssemblyName>` to add the required `.SpecFlowPlugin` at the end. You can also name your project with  `.SpecFlowPlugin` at the end.
`<GeneratePackageOnBuild>` is set to true, so that the NuGet package is generated on build.
We use a NuSpec file (SamplePlugin.nuspec) to provde all information for the NuGet package. This is set with the `<NuspecFile>` property.

For a Runtime plugin you only need a reference to the `SpecFlow`- NuGet package.

#### build/SpecFlow.SamplePlugin.targets

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/build/SpecFlow.SamplePlugin.targets>

We need to copy a different assembly to the output folder depending on the target framework of the project using the runtime plugin package.
Because the `$(TargetFrameworkIdentifider)` property is only availabe in imported `target` files, we have to work out the path to the assembly here.

#### build/SpecFlow.SamplePlugin.props

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/build/SpecFlow.SamplePlugin.props>

To copy the plugin assembly to the output folder, we include it in the `None`- ItemGroup and set `CopyToOutputDirectory` to `PreserveNewest`. This ensures that it is still copied to the output directory also if you change it.

#### Directory.Build.targets

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/Directory.Build.targets>

To specify the path to the assemblies which should be included in the NuGet package, we have to set various NuSpec properties.
This is done in the `Directory.Build.targets`, so it defined for all projects in subfolders. We add the value of the current configuration to the available properties.

#### SamplePlugin.nuspec

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/SamplePlugin.nuspec>

This is the NuSpec filethat privodes information for the NuGet package. To get the files from the build directory into the NuGet package, we have to specify them in the `file` list.
The runtime plugin assemblies are also specified here, using the additional `$config$` property we added in the `Directory.Build.targets`.

## Generator plugin

Generator plugins need to target .NET Framework 4.7.1 and .NET Standard 2.0.
The MSBuild task needs to know which generator plugins it should use. You therfore have to add your generator plugin to the `SpecFlowGeneratorPlugins` ItemGroup.
This is passed to the MSBuild task as a parameter and later used to load the plugins.

### Sample generator plugin

A complete example of a generator plugin can be found here: <https://github.com/techtalk/SpecFlow-Examples/tree/master/Plugins/GeneratorOnlyPlugin>
It packages a Generator plugin into a NuGet package.

#### SampleGeneratorPlugin.csproj

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/SampleGeneratorPlugin.csproj>

The project targets multiple frameworks, so it uses `<TargetFrameworks>` and not `<TargetFramework>`.
We set a different `<AssemblyName>` to add the required `.SpecFlowPlugin` at the end. You can also name your project with  `.SpecFlowPlugin` at the end.
`<GeneratePackageOnBuild>` is set to true, so that the NuGet package is generated on build.
We use a NuSpec file (SamplePlugin.nuspec) to provde all information for the NuGet package. This is set with the `<NuspecFile>` property.

For a generator plugin you  need a reference to the `SpecFlow.CustomPlugin`- NuGet package.

#### build/SpecFlow.SamplePlugin.targets

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/build/SpecFlow.SamplePlugin.targets>

We have to add a different assembly to the ItemGroup depending on the MSBuild version in use (.NET Full Framework or .NET Core).
Because the `$(MSBuildRuntimeType)` property is only availabe in imported `target` files, we have to work out the path to the assembly here.

#### build/SpecFlow.SamplePlugin.props

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/build/SpecFlow.SamplePlugin.props>

As we have now a property containing the path to the assembly, we can add it to the `SpecFlowGeneratorPlugins` ItemGroup here.

#### Directory.Build.targets

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/Directory.Build.targets>

To specify the path to the assemblies which should be included in the NuGet package, we have to set various NuSpec properties.
This is done in the `Directory.Build.targets`, so it defined for all projects in subfolders. We add the value of the current configuration to the available properties.

#### SamplePlugin.nuspec

<https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/SamplePlugin.nuspec>

This is the NuSpec filethat privodes information for the NuGet package. To get the files from the build directory into the NuGet package, we have to specify them in the `file` list.
The generator plugin assemblies are also specified here, using the additional `$config$` property we added in the `Directory.Build.targets`.
It is important to ensure that they are not added to the `lib` folder. If this were the case, they would be referenced by the project where you add the NuGet package. This is something we don't want to happen!

## Combined Package with both plugins

If you need to update generator and runtime plugisn with a single NuGet package (as we are doing with the `SpecFlow.xUnit`, `SpecFlow.NUnit` and `SpecFlow.xUnit` packages), you can do so.

As with the seperate plugins, you need two projects. One for the runtime plugin, and one for the generator plugin. As you only want one NuGet package, the** NuSpec files must only be present in the generator project**.
This is because the generator plugin is built with a higher .NET Framework version (.NET 4.7.1), meaning you can add a dependency on the Runtime plugin (which is only .NET 4.5). This will not working the other way around.

You can simply combine the contents of the `.targets` and `.props` file to a single one.

### Example

A complete example of a NuGet package that contains a runtime and generator plugin can be found here: <https://github.com/techtalk/SpecFlow-Examples/tree/master/Plugins/CombinedPlugin>

The files do the same as above.
