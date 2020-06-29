# Bindings

The [Gherkin feature files](../Gherkin/Using-Gherkin-Language-in-SpecFlow.md) are closer to free-text than to code – they cannot be executed as they are. The automation that connects the specification to the application interface has to be developed first. The automation that connects the Gherkin specifications to source code is called a _binding_. The binding classes and methods can be defined in the SpecFlow project or in [external binding assemblies](Use Bindings from External Assemblies.md).

There are several kinds of bindings in SpecFlow. The most important one is the [step definition](Step-Definitions.md) that automates the scenario at the step level. This means that instead of providing automation for the entire scenario, it has to be done for each separate step. The benefit of this model is that the step definitions can be reused in other scenarios, making it possible to (partly) construct further scenarios from existing steps with less (or no) automation effort. See [Step Definitions](Step Definitions.md) for details.

SpecFlow supports the following advanced binding techniques:

1. [Hooks](Hooks.md) can be used to perform additional automation logic on specific events, e.g. before executing a scenario.

2. [Step argument transformations](Step-Argument-Conversions.md) can apply a transformation to arguments in step definitions. The step argument transformation is a method that provides a conversion from text (specified by a regular expression) to an arbitrary .NET type. 

3. [Scoped bindings](Scoped-Step-Definitions.md) can be used to restrict the scope of a step definition or hook using scope rules. Each scope rule can define restrictions based on the feature title, scenario title or tags. Scoped step bindings allow you to define different automation logic for the same step depending on where it is used.  

In many cases, the different bindings have to exchange data during execution. See [Sharing Data between Bindings](Sharing-Data-between-Bindings.md) for more information on doing so.
