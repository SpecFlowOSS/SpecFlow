# Bindings

The [Gherkin feature files](../Gherkin/Gherkin-Reference.md) are closer to free-text than to code – they cannot be executed as they are. The automation that connects the specification to the application interface has to be developed first. The automation that connects the Gherkin specifications to source code is called a _binding_. The binding classes and methods can be defined in the SpecFlow project or in [external binding assemblies](Use-Bindings-from-External-Assemblies.md).

There are several kinds of bindings in SpecFlow. 

## Step Definitions

This is the most important one. The [step definition](Step-Definitions.md) that automates the scenario at the step level. This means that instead of providing automation for the entire scenario, it has to be done for each separate step. The benefit of this model is that the step definitions can be reused in other scenarios, making it possible to (partly) construct further scenarios from existing steps with less (or no) automation effort.  

It is required to add the `[Binding]` attribute to the classes where you define your step definitions.

## Hooks

[Hooks](Hooks.md) can be used to perform additional automation logic on specific events, e.g. before executing a scenario.
