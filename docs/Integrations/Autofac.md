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
### 2.  Create a static method somewhere in the SpecFlow project  
  (Recommended to put it into the `Support` folder) that returns an Autofac `ContainerBuilder` and tag it with the `[ScenarioDependencies]` attribute. 
  ##### 2.1 Configure your dependencies for the scenario execution within the method. 
  ##### 2.2 You also have to register the step definition classes, that you can do by either registering all public types from the SpecFlow project:

```csharp
builder.RegisterAssemblyTypes(typeof(YourClassInTheSpecFlowProject).Assembly).SingleInstance();
```
  ##### 2.3 or by registering all classes marked with the `[Binding]` attribute:

```csharp
builder.RegisterTypes(typeof(TestDependencies).Assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(BindingAttribute))).ToArray()).SingleInstance();
```
  ### 3. A typical dependency builder method probably looks like this:

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
