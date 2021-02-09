# Configuration

SpecFlow's behavior can be configured extensively. How to configure SpecFlow depends on the version of SpecFlow you are using.

## SpecFlow 3.x

Starting with SpecFlow 3, you can use the `specflow.json` file to configure it. It is mandatory for .NET Core projects and it is recommended for .NET Framework projects.  
When using the .NET Framework, you can still use the `app.config` file, as with earlier versions of SpecFlow.

If both the `specflow.json` and `app.config` files are available in a project, `specflow.json` takes precedence.

Please make sure that the **Copy to Output Directory property** of `specflow.json` is set to either **Copy always** or **Copy if newer**. Otherwise `specflow.json` might not get copied to the Output Directory, which results in the configuration specified in `specflow.json` taking no effect during text execution.

## SpecFlow 2.x

SpecFlow 2 is configured in your standard .NET configuration file, `app.config`, which is automatically added to your project. This method is not supported by .NET Core, and SpecFlow 2 does not include .NET Core support.

We recommend using `specflow.json` in new projects.

## Configuration Options

Both configuration methods use the same options and general structure. The only difference is that SpecFlow 2 only uses the `app.config` file (XML) and SpecFlow 3 requires the `specflow.json` file (JSON) for .NET Core projects.

### Configuration examples

The following 2 examples show the same option defined in the `specflow.json` and `app.config` in formats:

**specflow.json example:**

```json
{
  "language": {
    "feature": "de-AT"
  }
}
```

**app.config example:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="specFlow" type="TechTalk.SpecFlow.Configuration.ConfigurationSectionHandler, TechTalk.SpecFlow" />
  </configSections>
  <specFlow>
    <language feature="de-AT" />
  </specFlow>
</configuration>
```

You can find more examples in the [sample projects](https://github.com/techtalk/SpecFlow-Examples) for SpecFlow.

## Default Configuration

All SpecFlow configuration options have a default setting. Simple SpecFlow projects may not require any further configuration.

## Configuring Your Unit Test Provider

### SpecFlow 3

You can only configure your unit provider by adding the corresponding packages to your project. You will therefore need to add **one** of the following NuGet packages to your project to configure the unit test provider:

- SpecRun.SpecFlow
- SpecFlow.xUnit
- SpecFlow.MsTest
- SpecFlow.NUnit

**Note: Make sure you do not add more than one of the unit test plugins to your project. If you do, an error message will be displayed.**

### SpecFlow 2

SpecFlow 2 is configured using the `app.config` file (Full Framework only). Enter your unit test provider in the `unitTestProvider` element in the `specflow` section, e.g.:

```xml
  <specFlow>
    <unitTestProvider name="MsTest" />
  </specFlow>
```

## Configuration Elements

The same configuration elements are available in both the XML (`app.config`) and JSON (`specflow.json`) formats.

### `language`

Use this section to define the default language for feature files and other language-related settings. For more details on language settings, see [Feature Language](../Gherkin/Feature-Language.md).

| Attribute | Value                  | Description                                                                                                                                                                                                                                                             |
| --------- | ---------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| feature   | culture name (“en-US”) | The default language of feature files added to the project. We recommend using specific culture names (e.g.: “en-US”) rather than generic (neutral) cultures (e.g.: “en”). <br/> **Default:** en-US                                                                     |
| tool      | empty or culture name  | Specifies the language that SpecFlow uses for messages and tracing. Uses the default feature language if empty and that language is supported; otherwise messages are displayed in English. (<b>Note:</b> Only English is currently supported.)<br/> **Default:** empty |

### `bindingCulture`

Use this section to define the culture for executing binding methods and converting step arguments. For more details on language settings, see [Feature Language](../Gherkin/Feature-Language.md).

| Attribute | Value                  | Description                                                                                                                                                             |
| --------- | ---------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| name      | culture name (“en-US”) | Specifies the culture to be used to execute binding methods and convert step arguments. If not specified, the feature language is used.<br/> **Default:** not specified |

### `generator`

Use this section to define unit test generation options.

| Attribute                | Value      | Description                                                                                                                                                                                                                                          |
| ------------------------ | ---------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| allowDebugGeneratedFiles | true/false | By default, the debugger is configured to step through the generated code. This helps you debug your feature files and bindings (see [Debugging Tests]()). Disabled this option by setting this attribute to “true”.<br/> **Default:** false         |
| allowRowTests            | true/false | Determines whether "row tests" should be generated for [scenario outlines](../Gherkin/Gherkin-Reference.md). This setting is ignored if the [unit test framework](Unit-Test-Providers.md) does not support row based testing.<br/> **Default:** true |

### `runtime`

Use this section to specify various test execution options.

| Attribute                    | Value                     | Description                                                                                                                                                                                                |
| ---------------------------- | ------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| dependencies                 | custom dependencies       | Specifies custom dependencies for the SpecFlow runtime. See [Plugins](../Extend/Plugins.md) for details.<br/>**Default:** not specified                                                                    |
| missingOrPendingStepsOutcome | Inconclusive/Ignore/Error | Determines how SpecFlow behaves if a step binding is not implemented or pending. See [Test Result](../Execution/Test-Results.md).<br/> **Default:** Inconclusive                                           |
| obsoleteBehavior             | None/Warn/Pending/Error   | how SpecFlow behaves if a step binding is marked with [Obsolete] attribute.<br/> **Default:** Warn                                                                                                         |
| stopAtFirstError             | true/false                | Determines whether the execution should stop when encountering the first error, or whether it should attempt to try and match subsequent steps (in order to detect missing steps).<br/> **Default:** false |

### `trace`

Use this section to determine the SpecFlow trace output.

| Attribute                   | Value                                                                     | Description                                                                                                                                                                    |
| --------------------------- | ------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| traceSuccessfulSteps        | true/false                                                                | Determines whether SpecFlow should trace successful step binding executions. <br/>**Default:** true                                                                            |
| traceTimings                | true/false                                                                | Determines whether SpecFlow should trace execution time of the binding methods (only if the execution time is longer than the minTracedDuration value).<br/>**Default:** false |
| minTracedDuration           | TimeSpan (0:0:0.1)                                                        | Specifies a threshold for tracing the binding execution times.<br/>**Default:** 0:0:0.1 (100 ms)                                                                               |
| stepDefinitionSkeletonStyle | RegexAttribute/MethodNameUnderscores/MethodNamePascalCase/MethodNameRegex | Specifies the default [step definition style](../Bindings/Step-Definitions.html#step-matching-styles-rules).<br/>**Default:** RegexAttribute                                   |

### `stepAssemblies`

This section can be used to configure additional assemblies that contain [external binding assemblies](../Bindings/Use-Bindings-from-External-Assemblies.md). The assembly of the SpecFlow project (the project containing the feature files) is automatically included. The binding assemblies must be placed in the output folder (e.g. bin/Debug) of the SpecFlow project, for example by adding a reference to the assembly from the project.

The following example registers an additional binding assembly (MySharedBindings.dll).

**specflow.json example:**

```json
{
  "stepAssemblies": [
    {
      "assembly": "MySharedBindings"
    }
  ]
}
```

**app.config example:**

```xml
<specFlow>
  <stepAssemblies>
    <stepAssembly assembly="MySharedBindings" />
  </stepAssemblies>
</specFlow>
```

The `<stepAssemblies>` can contain multiple `<stepAssembly>` elements (one for each assembly), with the following attributes.

| Attribute | Value         | Description                                   |
| --------- | ------------- | --------------------------------------------- |
| assembly  | assembly name | The name of the assembly containing bindings. |
