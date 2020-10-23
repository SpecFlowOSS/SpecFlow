# Plugins (Legacy)

## Introduction

SpecFlow provides a plugin infrastructure, allowing customization. You can develop SpecFlow plugins that change the behavior of the built-in generator and runtime components. For example, a plugin could provide support for a new unit testing framework.

To use a custom plugin it has to be enabled in the [[configuration]] (app.config) of your SpecFlow project:

```xml
<specFlow>
  <plugins>
    <add name="MyPlugin" />
  </plugins>
</specFlow>
```

## Creating plugins

Creating a plugin is fairly simple. There are 3 types of plugins supported:

* Runtime
* Generator
* Runtime Generator

By default, SpecFlow assumes plugins are Runtime Generator plugins. If your plugin is either a Runtime or Generator plugin, you need to add this information to the configuration.

Example for a Runtime plugin:
```xml
<specFlow>
  <plugins>
    <add name="MyPlugin" type="Runtime"/>
  </plugins>
</specFlow>
```

Example for a Generator plugin:
```xml
<specFlow>
  <plugins>
    <add name="MyPlugin" type="Generator"/>
  </plugins>
</specFlow>
```

The steps required to create plugins of all types are similar. **All plugins require the suffix ".SpecFlowPlugin".**

### Generator Plugin

**Needed steps for creating a Generator Plugin**

1. A SpecFlow.CustomPlugin Nuget package added to the library that will contain the plugin.
2. A class that implements `IGeneratorPlugin` interface (which is defined in TechTalk.SpecFlow.Generator.Plugins namespace)
3. An assembly level attribute `GeneratorPlugin` pointing to the class that implements `IGeneratorPlugin`

Let's analyze all of these steps in detail.

I will advise you start a new class library for each plugin you intend to create. Once you create your class library as a first thing you should add the SpecFlow.CustomPlugin Nuget package to your project.
Once this is done, you need to define a class that will represent your plugin. For this class in order to be seen as a SpecFlow plugin, it needs to implement the `IGeneratorPlugin` interface.
By implementing the `Initialize`- Method on the `IGeneratorPlugin` interface, you get access to the GeneratorPluginEvents and GeneratorPluginParameters.

GeneratorPluginEvents

* *ConfigurationDefaults* – If you are planning to intervene at the SpecFlow configuration, this is the right place to get started.
* *CustomizeDependencies* – If you are extending any of the components of SpecFlow, you can register your implementation at this stage.
* *RegisterDependencies* – In case your plugin is of a complex nature and it has it’s own dependencies, this can be the right place to set your Composition Root.

As an example, if you are writing a plugin that will act as unit test generator provider, you are going to register it in the following way, inside the *CustomizeDependencies* event handler:

```csharp
public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters)
{
        generatorPluginEvents.CustomizeDependencies += CustomizeDependencies;
}

public void CustomizeDependencies(CustomizeDependenciesEventArgs eventArgs)
{
	eventArgs.ObjectContainer.RegisterTypeAs<MyNewGeneratorProvider, IUnitTestGeneratorProvider>();
}
```

In order for your new library to be picked up by SpecFlow plugin loader, you need to flag your assembly with the `GeneratorPlugin` attribute. This is an example of it, taking in consideration that  the class that implements `IGeneratorPlugin` interface is called `MyNewPlugin`.
```csharp
[assembly: GeneratorPlugin(typeof(MyNewPlugin))]
```

### Runtime Plugin

**Needed steps for creating a Runtime Plugin**

1. A SpecFlow.CustomPlugin Nuget package added to the library that will contain the plugin.
2. A class that implements `IRuntimePlugin` interface (which is defined in TechTalk.SpecFlow.Plugins namespace)
3. An assembly level attribute `RuntimePlugin` pointing to the class that implements `IRuntimePlugin`

By implementing the `Initialize`- Method on the `IRuntimePlugin` interface, you get access to the RuntimePluginEvents and RuntimePluginParameters.

**RuntimePluginsEvents**

* *RegisterGlobalDependencies* - register new interfaces to the global container, see <a href="https://github.com/techtalk/SpecFlow/wiki/Available-Containers-&-Registrations">Available-Containers-&-Registrations</a>
* *CustomizeGlobalDependencies* - override registrations in the global container, see <a href="https://github.com/techtalk/SpecFlow/wiki/Available-Containers-&-Registrations">Available-Containers-&-Registrations</a>
* *ConfigurationDefaults* - adjust configuration values
* *CustomizeTestThreadDependencies* - override or register new interfaces in the test thread container, see <a href="https://github.com/techtalk/SpecFlow/wiki/Available-Containers-&-Registrations">Available-Containers-&-Registrations</a>
* *CustomizeFeatureDependencies* - override or register new interfaces in the feature container, see <a href="https://github.com/techtalk/SpecFlow/wiki/Available-Containers-&-Registrations">Available-Containers-&-Registrations</a>
* *CustomizeScenarioDependencies* - override or register new interfaces in the scenario container, see <a href="https://github.com/techtalk/SpecFlow/wiki/Available-Containers-&-Registrations">Available-Containers-&-Registrations</a>

In order for your new library to be picked up by SpecFlow plugin loader, you need to flag your assembly with the `RuntimePlugin` attribute. This is an example of it, taking in consideration that the class that implements `IRuntimePlugin` interface is called `MyNewPlugin`.
```csharp
[assembly: RuntimePlugin(typeof(MyNewPlugin))]
```

Note: Parameters are not yet implemented (Version 2.1)

## Configuration details

In order to load your plugin, in your SpecFlow project, you need to reference your plugin in the app.config file without the ".SpecFlowPlugin" suffix. It is also handy to know that the path attribute considers that project root as a path root. The following example is used to load a plugin assembly called "MyNewPlugin.SpecFlowPlugin.dll" that is located in a folder called "Binaries" that is at the same level of the current project.

```xml
<specFlow>
	<plugins>
		<add name="MyNewPlugin" path="..\Binaries" />
	</plugins>
</specFlow>
```
## Sample plugin implementations:

For reference, here are some sample implementations of an IRuntimePlugin and IGeneratorPlugin:

[SpecFlow.FsCheck](https://github.com/gasparnagy/SpecFlow.FsCheck/blob/master/src/SpecFlow.FsCheck.SpecFlowPlugin/FsCheckPlugin.cs)

[SpecFlow.Autofac](https://github.com/phatcher/SpecFlow.Unity/blob/master/code/SpecFlow.Unity.SpecFlowPlugin/UnityPlugin.cs)
