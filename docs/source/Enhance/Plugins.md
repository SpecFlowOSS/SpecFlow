# Plugins

## Changes with SpecFlow 3

This information only applies to SpecFlow 3. For legacy information on plugins for previous versions, see [[Plugins (Legacy)]]. With SpecFlow 3, we have changed how you configure which plugins are used. They are no longer configured in your `app.config` (or `specflow.json`).

## Runtime plugins

Runtime plugins need to target .NET Framework 4.5 and .NET Standard 2.0.
SpecFlow searches for files that end with `.SpecFlowPlugin.dll` in the following locations:

* The folder containing your `TechTalk.SpecFlow.dll` file
* Your working directory

SpecFlow loads plugins in the order they are found in the folder.

### Sample runtime plugin

A complete example of a Runtime plugin can be found [[here|https://github.com/techtalk/SpecFlow-Examples/tree/master/Plugins/RuntimeOnlyPlugin]]. It packages a Runtime plugin as a NuGet package.

#### SampleRuntimePlugin.csproj

The sample project is [[here|https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/SampleRuntimePlugin.csproj]].

This project targets multiple frameworks, so the project file uses `<TargetFrameworks>` instead of `<TargetFramework>`. Our target frameworks are .NET 4.5 and .NET Standard 2.0.
```
<TargetFrameworks>net45;netstandard2.0</TargetFrameworks>
```

We set a different `<AssemblyName>` to add the required `.SpecFlowPlugin` suffix to the assembly name. You can also simply name your project with  `.SpecFlowPlugin` at the end.
```
<AssemblyName>SampleRuntimePlugin.SpecFlowPlugin</AssemblyName>
```

`<GeneratePackageOnBuild>` is set to true so that the NuGet package is generated on build.
We use a NuSpec file (SamplePlugin.nuspec) to provde all information for the NuGet package. This is set with the `<NuspecFile>` property.
```
<GeneratePackageOnBuild>true</GeneratePackageOnBuild>
<NuspecFile>$(MSBuildThisFileDirectory)SamplePlugin.nuspec</NuspecFile>
```

Runtime plugins only need a reference to the `SpecFlow` NuGet package.
```
<ItemGroup>
    <PackageReference Include="SpecFlow" Version="3.0.199" />
</ItemGroup>
```

#### build/SpecFlow.SamplePlugin.targets

The sample targets file is [[here|https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/build/SpecFlow.SamplePlugin.targets]].

We need to copy a different assembly to the output folder depending on the target framework (.NET Core vs .NET Framework) of the project using the runtime plugin package. Because the `$(TargetFrameworkIdentifider)` property is only availabe in imported `targets` files, we have to work out the path to the assembly here.
```
<_SampleRuntimePluginFramework Condition=" '$(TargetFrameworkIdentifier)' == '.NETCoreApp' ">netstandard2.0</_SampleRuntimePluginFramework>
<_SampleRuntimePluginFramework Condition=" '$(TargetFrameworkIdentifier)' == '.NETFramework' ">net45</_SampleRuntimePluginFramework>
<_SampleRuntimePluginPath>$(MSBuildThisFileDirectory)\..\lib\$(_SampleRuntimePluginFramework)\SampleRuntimePlugin.SpecFlowPlugin.dll</_SampleRuntimePluginPath>
```

#### build/SpecFlow.SamplePlugin.props

The sample props file is [[here|https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/build/SpecFlow.SamplePlugin.props]].

To copy the plugin assembly to the output folder, we include it in the `None` ItemGroup and set `CopyToOutputDirectory` to `PreserveNewest`. This ensures that it is still copied to the output directory even if you change it.

```
<ItemGroup>
    <None Include="$(_SampleRuntimePluginPath)" >
        <Link>%(Filename)%(Extension)</Link>
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        <Visible>False</Visible>
    </None>
</ItemGroup>
```

#### Directory.Build.targets

The sample file is [[here|https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/Directory.Build.targets]].

To specify the path to the assemblies which should be included in the NuGet package, we have to set various NuSpec properties.
This is done in the `Directory.Build.targets`, so it defined for all projects in subfolders. We add the value of the current configuration to the available properties.

```
<Target Name="SetNuspecProperties" BeforeTargets="GenerateNuspec" >
    <PropertyGroup>
        <NuspecProperties>$(NuspecProperties);config=$(Configuration)</NuspecProperties>
    </PropertyGroup>
</Target>
```

#### SamplePlugin.nuspec

The sample file is [[here|https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/RuntimeOnlyPlugin/RuntimePlugin/SamplePlugin.nuspec]].

This is the NuSpec filethat provides information on the NuGet package. To get the files from the build directory into the NuGet package, we have to specify them in the `file` list.
The runtime plugin assemblies are also specified here, using the additional `$config$` property we added in the `Directory.Build.targets`.

<files>
    <file src="build\**\*" target="build"/>
    <file src="bin\$config$\net45\SampleRuntimePlugin.SpecFlowPlugin.*" target="lib\net45"/>
    <file src="bin\$config$\netstandard2.0\SampleRuntimePlugin.SpecFlowPlugin.dll" target="lib\netstandard2.0"/>
    <file src="bin\$config$\netstandard2.0\SampleRuntimePlugin.SpecFlowPlugin.pdb" target="lib\netstandard2.0"/>
</files>

## Generator plugins

Generator plugins need to target .NET Framework 4.7.1 and .NET Standard 2.0.
The MSBuild task needs to know which generator plugins it should use. You therfore have to add your generator plugin to the `SpecFlowGeneratorPlugins` ItemGroup.
This is passed to the MSBuild task as a parameter and later used to load the plugins.

### Sample generator plugin

A complete example of a generator plugin can be found [[here|https://github.com/techtalk/SpecFlow-Examples/tree/master/Plugins/GeneratorOnlyPlugin]]. It packages a Generator plugin into a NuGet package.

#### SampleGeneratorPlugin.csproj

The sample project is [[here|https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/SampleGeneratorPlugin.csproj]].

The project targets multiple frameworks, so it uses `<TargetFrameworks>` and not `<TargetFramework>`.
We set a different `<AssemblyName>` to add the required `.SpecFlowPlugin` at the end. You can also name your project with  `.SpecFlowPlugin` at the end.
`<GeneratePackageOnBuild>` is set to true, so that the NuGet package is generated on build.
We use a NuSpec file (SamplePlugin.nuspec) to provde all information for the NuGet package. This is set with the `<NuspecFile>` property.
```
<PropertyGroup>
    <TargetFrameworks>net471;netstandard2.0</TargetFrameworks>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <NuspecFile>$(MSBuildThisFileDirectory)SamplePlugin.nuspec</NuspecFile>
    <AssemblyName>SampleGeneratorPlugin.SpecFlowPlugin</AssemblyName>
</PropertyGroup>
```

For a generator plugin you  need a reference to the `SpecFlow.CustomPlugin`- NuGet package.

```
<ItemGroup>
    <PackageReference Include="SpecFlow.CustomPlugin" Version="3.0.199" />
</ItemGroup>
```

#### build/SpecFlow.SamplePlugin.targets

The sample targets file is [[here|https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/build/SpecFlow.SamplePlugin.targets]].

We have to add a different assembly to the ItemGroup depending on the MSBuild version in use (.NET Full Framework or .NET Core).
Because the `$(MSBuildRuntimeType)` property is only availabe in imported `target` files, we have to work out the path to the assembly here.

```
<PropertyGroup>
    <_SampleGeneratorPluginFramework Condition=" '$(MSBuildRuntimeType)' == 'Core'">netstandard2.0</_SampleGeneratorPluginFramework>
    <_SampleGeneratorPluginFramework Condition=" '$(MSBuildRuntimeType)' != 'Core'">net471</_SampleGeneratorPluginFramework>
    <_SampleGeneratorPluginPath>$(MSBuildThisFileDirectory)\$(_SampleGeneratorPluginFramework)\SampleGeneratorPlugin.SpecFlowPlugin.dll</_SampleGeneratorPluginPath>
</PropertyGroup>
```

#### build/SpecFlow.SamplePlugin.props

The sample props file is [[here|https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/build/SpecFlow.SamplePlugin.props]].

As we have now a property containing the path to the assembly, we can add it to the `SpecFlowGeneratorPlugins` ItemGroup here.

```
<ItemGroup>
    <SpecFlowGeneratorPlugins Include="$(_SampleGeneratorPluginPath)" />
</ItemGroup>
```

#### Directory.Build.targets

The sample file is [[here|https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/Directory.Build.targets]].

To specify the path to the assemblies which should be included in the NuGet package, we have to set various NuSpec properties.
This is done in `Directory.Build.targets`, so it defined for all projects in subfolders. We add the value of the current configuration to the available properties.

```
<Target Name="SetNuspecProperties" BeforeTargets="GenerateNuspec" >
    <PropertyGroup>
        <NuspecProperties>$(NuspecProperties);config=$(Configuration)</NuspecProperties>
    </PropertyGroup>
</Target>
```

#### SamplePlugin.nuspec

The sample file is [[here|https://github.com/techtalk/SpecFlow-Examples/blob/master/Plugins/GeneratorOnlyPlugin/GeneratorPlugin/SamplePlugin.nuspec]].

This is the NuSpec filethat privodes information for the NuGet package. To get the files from the build directory into the NuGet package, we have to specify them in the `file` list.
The generator plugin assemblies are also specified here, using the additional `$config$` property we added in the `Directory.Build.targets`.
It is important to ensure that they are not added to the `lib` folder. If this were the case, they would be referenced by the project where you add the NuGet package. This is something we don't want to happen!

```
<files>
    <file src="build\**\*" target="build"/>
    <file src="bin\$config$\net471\SampleGeneratorPlugin.SpecFlowPlugin.*" target="build\net471"/>
    <file src="bin\$config$\netstandard2.0\SampleGeneratorPlugin.SpecFlowPlugin.dll" target="build\netstandard2.0"/>
    <file src="bin\$config$\netstandard2.0\SampleGeneratorPlugin.SpecFlowPlugin.pdb" target="build\netstandard2.0"/>
</files>
```

## Combined Package with both plugins

If you need to update generator and runtime plugins with a single NuGet package (as we are doing with the `SpecFlow.xUnit`, `SpecFlow.NUnit` and `SpecFlow.xUnit` packages), you can do so.

As with the separate plugins, you need two projects. One for the runtime plugin, and one for the generator plugin. As you only want one NuGet package, the **NuSpec files must only be present in the generator project**.
This is because the generator plugin is built with a higher .NET Framework version (.NET 4.7.1), meaning you can add a dependency on the Runtime plugin (which is only .NET 4.5). This will not working the other way around.

You can simply combine the contents of the `.targets` and `.props` file to a single one.

### Example

A complete example of a NuGet package that contains a runtime and generator plugin can be found [[here|https://github.com/techtalk/SpecFlow-Examples/tree/master/Plugins/CombinedPlugin]].


## Plugin Developer Channel
We have set up a Gitter channel for plugin developers here. If you questions regarding the development of plugins for SpecFlow, this is the place to ask them.