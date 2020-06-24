.. toctree::
    :maxdepth: 1
    :hidden:
    :name: bindings

    Step Definitions.md
    Scoped Step Definitions.md
    Asynchronous Bindings.md
    
    Hooks.md
    
    Sharing Data between Bindings.md
    Context-Injection.md
    ScenarioContext.md
    FeatureContext.md
    Calling-Steps-from-Step-Definitions.md

    Step Argument Transformations.md
    Step Argument Conversions.md

    Use Bindings from External Assemblies.md

    Renaming-Steps.md
    SpecFlow-Assist-Helpers.md
    FSharp-Support.md

The [[Gherkin feature files|Using Gherkin Language in SpecFlow]] are closer to free-text than to code – they cannot be executed as they are. The automation that connects the specification to the application interface has to be developed first. The automation that connects the Gherkin specifications to source code is called a _binding_. The binding classes and methods can be defined in the SpecFlow project or in [[external binding assemblies|Use Bindings from External Assemblies]].

There are several kinds of bindings in SpecFlow. The most important one is the [[step definition|Step Definitions]] that automates the scenario at the step level. This means that instead of providing automation for the entire scenario, it has to be done for each separate step. The benefit of this model is that the step definitions can be reused in other scenarios, making it possible to (partly) construct further scenarios from existing steps with less (or no) automation effort. See [[Step Definitions]] for details.

SpecFlow supports the following advanced binding techniques:

1. [[Hooks]] can be used to perform additional automation logic on specific events, e.g. before executing a scenario.

2. [[Step argument transformations|Step Argument Conversions]] can apply a transformation to arguments in step definitions. The step argument transformation is a method that provides a conversion from text (specified by a regular expression) to an arbitrary .NET type. 

3. [[Scoped bindings]] can be used to restrict the scope of a step definition or hook using scope rules. Each scope rule can define restrictions based on the feature title, scenario title or tags. Scoped step bindings allow you to define different automation logic for the same step depending on where it is used. 

In many cases, the different bindings have to exchange data during execution. See [[Sharing Data between Bindings]] for more information on doing so.