# Castle Windsor

## Introduction
SpecFlow plugin for using Castle Windsor as a dependency injection framework for step definitions.

***Currently supports***

**Castle Windsor v5.0.1 or above**

## Step by step walkthrough of using SpecFlow.Windsor

### 1.  Install plugin from NuGet into your SpecFlow project.

```csharp
PM> Install-Package SpecFlow.Windsor
```
### 2.  Create a static method somewhere in the SpecFlow project  
  (Recommended to put it into the `Support` folder) that returns a Windsor `IWindsorContainer` and tag it with the `[ScenarioDependencies]` attribute. 
  ##### 2.1 Configure your dependencies for the scenario execution within the method. 
  ##### 2.2 All your binding classes are automatically registered, including ScenarioContext etc.

### 3. A typical dependency builder method probably looks like this:
```csharp
[ScenarioDependencies]
public static IWindsorContainer CreateContainer()
{
  var container = new WindsorContainer();

  //TODO: add customizations, stubs required for testing

  return container;
}
```

### 4. To re-use a container betweeen scenarios, try the following:
Your shared services will be resolved from the root container, while scoped objects
such as ScenarioContext will be resolved from the new container.
```csharp
[ScenarioDependencies]
public static IWindsorContainer CreateContainer()
{
  var container = new WindsorContainer();
  container.Parent = sharedRootContainer;

  return container;
}
```

### 5. To customize binding behavior, use the following:
Default behavior is to auto-register bindings. To manually register these during `CreateContainer`
you can use the following attribute:

```csharp
[ScenarioDependencies(AutoRegisterBindings = false)]
public static IWindsorContainer CreateContainer()
{
    // Register your bindings here
}
```