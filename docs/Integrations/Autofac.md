# Autofac

## Introduction
SpecFlow plugin for using Autofac as a dependency injection framework for step definitions.

***Currently supports***

**Autofac v4.0.0 or above**

## Step by step walkthrough of using SpecFlow.Autofac


### 1.  Install plugin from NuGet into your SpecFlow project.

```csharp
PM> Install-Package SpecFlow.Autofac
```
### 2. There are two registration scenarios:
  
  ##### 2.1A When there are some dependecies that need to be shared globally for all scenarios:
  
  Create a static method somewhere in the SpecFlow project to register global dependencies: 
  (Recommended to put it into the `Support` folder) that returns an Autofac `IContainer` and tag it with the `[GlobalDependencies]` attribute. 

  Create a static method somewhere in the SpecFlow project to register scenario dependencies: 
  (Recommended to put it into the `Support` folder) that returns `void` and and has one parameter of Autofac `ContainerBuilder` tag it with the `[ScenarioDependencies]` attribute. 

  ##### 2.1B When dependencies are only recquired to be resolved each time for a scenario:
  
  Create a static method somewhere in the SpecFlow project  
  (Recommended to put it into the `Support` folder) that returns an Autofac `ContainerBuilder` and tag it with the `[ScenarioDependencies]` attribute. 

  ##### 2.2 Configure your dependencies for the scenario execution within either the 2 methods or the 1 method, the two solutions cannot be used together. 
  ##### 2.3 You also have to register the step definition classes in the `[ScenarioDependencies]` method, that you can do by either registering all public types from the SpecFlow project:

```csharp
builder.RegisterAssemblyTypes(typeof(YourClassInTheSpecFlowProject).Assembly).SingleInstance();
```
  ##### 2.4 or by registering all classes marked with the `[Binding]` attribute:

```csharp
builder.RegisterTypes(typeof(TestDependencies).Assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(BindingAttribute))).ToArray()).SingleInstance();
```
  ### 3. A typical dependency builder method for `[ScenarioDependencies]` probably looks like this:

```csharp
[GlobalDependencies]
public static IContainer CreateContainer()
{
    // create container with the runtime dependencies that will be available to all scenarios.
    var builder = Dependencies.CreateGlobalContainerBuilder();
    var container = builder.Build();
    return builder.Build();
}

[ScenarioDependencies]
public static void CreateContainerBuilder(ContainerBuilder builder)
{
  // create container with the runtime dependencies
  var builder = Dependencies.RegisterScenarioDependencies(builder);

  //TODO: add customizations, stubs required for testing

  builder.RegisterTypes(typeof(TestDependencies).Assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(BindingAttribute))).ToArray()).SingleInstance();
  
  return builder;
}
```


  ### 4. A typical dependency builder method for `[ScenarioDependencies]` probably looks like this:

```csharp
[ScenarioDependencies]
public static ContainerBuilder CreateContainerBuilder()
{
  // create container with the runtime dependencies
  var builder = Dependencies.CreateContainerBuilder();

  //TODO: add customizations, stubs required for testing

  builder.RegisterTypes(typeof(TestDependencies).Assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(BindingAttribute))).ToArray()).SingleInstance();
  
  return builder;
}
```
