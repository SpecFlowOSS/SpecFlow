# F# Support

[Bindings](Bindings.md) for SpecFlow can be written also in F#. Doing so you can take the advantages of the F# language for writing step definitions: you can define regex-named F# functions for your steps. Simply put the regex between double backticks.

```F#
let [<Given>] ``I have entered (.*) into the calculator``(number:int) = 
    Calculator.Push(number)
```

Although the regex method names are only important for [step definitions](Step-Definitions.md) you can also define [hooks](Hooks.md) and [step argument conversions](Step-Argument-Conversions.md) in the F# binding projects.

Note: You need to create a C# or VB project for hosting the feature files and configure your F# project(s) as [external binding assemblies](Use-Bindings-from-External-Assemblies.md):

```xml
<specFlow>
  <stepAssemblies>
    <stepAssembly assembly="MyFSharpBindings" />
  </stepAssemblies>
</specFlow>
```

## IDE Support

SpecFlow provides item templates for creating new F# step definitions or hooks in Visual Studio.

Note: The navigation and the binding analysis features of the SpecFlow editor provide only limited support for F# projects.

## Examples

An example can be found at <https://github.com/techtalk/SpecFlow-Examples/tree/master/BowlingKata/BowlingKata-Fsharp>