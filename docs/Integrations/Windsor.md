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
  ##### 2.2 You also have to register the step definition classes by registering all classes marked with the `[Binding]` attribute:

```csharp
container.Register(
  Types.FromAssembly(myAssembly)
    .Where(x => x.IsDefined(typeof(BindingAttribute), false))
    .LifestyleScoped());
```
  ### 3. A typical dependency builder method probably looks like this:

```csharp
[ScenarioDependencies]
public static IWindsorContainer CreateContainer()
{
  var container = new WindsorContainer();

  container.BeginScope();
  container.Register(
    Types.FromAssembly(myAssembly)
      .Where(x => x.IsDefined(typeof(BindingAttribute), false))
      .LifestyleScoped());
	  
  //TODO: add customizations, stubs required for testing

  return container;
}
```
