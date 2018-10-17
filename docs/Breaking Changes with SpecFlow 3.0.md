# Breaking changes with SpecFlow 3.0

## Supported test frameworks

We support following test frameworks/runners:

 - SpecFlow+Runner >= 3.0
 - NUnit >= 3.10
 - MSTest V2 >= 1.3.2
 - xUnit >= 2.4.0

## Unit Test Provider is configured via plugin

You don't configure the unittestprovider anymore in the app.config. This is done by the plugins for the various test frameworks.

So it is mandatory to use SpecRun.SpecFlow-3.0.0, SpecFlow.xUnit, SpecFlow.MsTest or SpecFlow.NUnit in your project to configure the unittestprovider.

## GeneratorPlugins are configured with MSBuild

Generator plugins aren't configured anymore in the app.config.
To get it loaded, you have to add it to the `SpecFlowGeneratorPlugins` item group via MSBuild.

The easiest way is, to package your Generator Plugin in a NuGet package and use the automatically import of the `props` and `target` files in them.
A good example are the plugins for the different test frameworks.

Let's have a look at the xUnit Plugin.  
The Generator Plugin is located at [/Plugins/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin](/Pluings/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin).

In the [/Plugins/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin/build/SpecFlow.xUnit.props](/Plugins/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin/build/SpecFlow.xUnit.props) you can add your entry to the `SpecFlowGeneratorPlugins` ItemGroup. Be carefull that dependend on your used MSBuild (Full Framework or .NET Core version), you have to put different assemblies in the ItemGroup.
The best way to do this, is in the [/Plugins/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin/build/SpecFlow.xUnit.targets](/Plugins/TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin/build/SpecFlow.xUnit.targets) files. You can use the `MSBuildRuntimeType` property to decide which assembly you want to use.

## RuntimePlugins are configured by References

Runtime plugins aren't configured anymore in the app.config also.
SpecFlow loads now all files in the folder of the test assembly and in the current working directory that ends with `.SpecFlowPlugin.dll`.

Because .NET Core doesn't copy references into the target directory, you have to add it to the `None` ItemGroup and set it's `CopyToOutputDirectory` to `PreserveNewest`.

You have to do the same decitions as with the generator plugins

## specflow.exe is gone

The specflow.exe was removed.  
To generate the code-behind files, please us the `SpecFlow.Tools.MsBuild.Generation` NuGet package.  

Reports where removed from the main code-base and aren't currently not available for SpecFlow 3.0. For details see GitHub Issue <https://github.com/techtalk/SpecFlow/issues/1036>

## Configuration app.config/specflow.json

Configuring SpecFlow via app.config is only available when you are using the Full Framework. If you are using .NET Core, you have to use the new specflow.json configuration file.
