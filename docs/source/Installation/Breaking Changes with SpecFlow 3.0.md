# Breaking changes with SpecFlow 3.0

## Supported test frameworks

We support the following test frameworks/runners:

 - SpecFlow+Runner >= 3.0
 - NUnit >= 3.10
 - MSTest V2 >= 1.3.2
 - xUnit >= 2.4.0

## Unit Test Provider is configured via plugin

The unittestprovider is no longer configured in app.config, but using plugins for the various test frameworks.

It is therefore mandatory to use SpecRun.SpecFlow-3.0.0, SpecFlow.xUnit, SpecFlow.MsTest or SpecFlow.NUnit in your project to configure the unittestprovider.

## GeneratorPlugins are configured with MSBuild

Generator plugins are no longer configured in app.config.
To load a plugin, you have to add it to the `SpecFlowGeneratorPlugins` item group via MSBuild.

The easiest way is to package your Generator Plugin in a NuGet package and use the automatic import of the `props` and `target` files in them.
A good example are the plugins for the different test frameworks.

For example, if we look at the xUnit Plugin:  
The Generator Plugin is located at [/Plugins/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin](https://github.com/SpecFlowOSS/SpecFlow/tree/master/Plugins/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin).

In the [/Plugins/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin/build/SpecFlow.xUnit.props](https://github.com/SpecFlowOSS/SpecFlow/tree/master/Plugins/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin/build/SpecFlow.xUnit.props) you can add your entry to the `SpecFlowGeneratorPlugins` ItemGroup. Be careful, because dependent on the version of MSBuild you use (Full Framework or .NET Core version), you have to put different assemblies in the ItemGroup.
The best way to do this is in the [/Plugins/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin/build/SpecFlow.xUnit.targets](https://github.com/SpecFlowOSS/SpecFlow/tree/master/Plugins/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin/build/SpecFlow.xUnit.targets) files. You can use the `MSBuildRuntimeType` property to decide which assembly you want to use.

## RuntimePlugins are configured by References

Runtime plugins are also no longer configuredin app.config.
SpecFlow loads now all files in the folder of the test assembly and in the current working directory that end with `.SpecFlowPlugin.dll`.

Because .NET Core doesn't copy references to the target directory, you have to add it to the `None` ItemGroup and set its `CopyToOutputDirectory` to `PreserveNewest`.

You have to do the same decitions <!-- What does this mean? --> as with the generator plugins

## specflow.exe is gone

The specflow.exe was removed.  
To generate the code-behind files, please use the `SpecFlow.Tools.MsBuild.Generation` NuGet package.  

Reports were removed from the main code-base and aren't currently available for SpecFlow 3.0. For details why we did it and where to find the extracted code is written in GitHub Issue <https://github.com/techtalk/SpecFlow/issues/1036>

## Configuration app.config/specflow.json

Configuring SpecFlow via app.config is only available when you are using the Full Framework. If you are using .NET Core, you have to use the new specflow.json configuration file.
