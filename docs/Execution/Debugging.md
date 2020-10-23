# Debugging

SpecFlow Visual Studio integration also supports debugging the execution of your tests. Just like in the source code files of your project, you can place breakpoints in the SpecFlow feature files. Whenever you execute the generated tests in debug mode, the execution will stop at the specified breakpoints and you can execute the steps one-by-one using “Step Over” (F10), or follow the detailed execution of the bindings using “Step Into” (F11). 

If the execution of a SpecFlow test is stopped at a certain point of the binding (e.g. because of an exception), you can navigate to the current step in the feature file from the “Call Stack” tool window in Visual Studio.

By default, you cannot debug inside the generated .feature.cs files. You can enable debugging for these files by setting [generator allowDebugGeneratedFiles="true"](../Installation/Configuration.md).