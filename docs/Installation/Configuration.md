# Configuration

SpecFlow's behaviour can be configured extensively. How to configure SpecFlow depends on the version of SpecFlow you are using. 

## SpecFlow 2.x

SpecFlow 2 is configured in your standard .NET configuration file, `app.config`, which is automatically added to your project. This method is not supported by .NET Core, and SpecFlow 2 does not include .NET Core support.

## SpecFlow 3.x

The configuration has been moved to a JSON file, `specflow.json`. This file is mandatory for .NET Core projects, and optional for projects using the Full Framework. When using the Full Framework, you can still use the `app.config` file, as with earlier versions of SpecFlow, or `specflow.json`. If both the `specflow.json` and `app.config` files are available in a Full Framework project, `specflow.json` takes precedence. 

We recommend using `specflow.json` in new projects.

## Configuration Options

Apart from selecting your unit test provider, both configuration methods use the same options and general structure. The only difference is that SpecFlow 2 only uses the `app.config` file (XML) and SpecFlow 3 requires the `specflow.json` file (JSON) for .NET Core projects.

Unit test providers for SpecFlow 3 are not configured in your configuration file, but by adding the appropriate packages to your solution, see Configuring Your Unit Test Provider below.

### Configuration examples

The following 2 examples show the same option defined in the `app.config` in `specflow.json` formats:

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


**specflow.json example:**
```
{
    "language": {
        "feature": "de-AT"
    }
}
```

You can find more examples in the [sample projects](https://github.com/techtalk/SpecFlow-Examples) for SpecFlow.

## Default Configuration

All SpecFlow configuration options have a default setting. When using the `app.config` method (Full Framework only), configuring your unit test provider (see below) is the most important configuration option required. Simple SpecFlow projects may not require any further configuration.

## Configuring Your Unit Test Provider

### SpecFlow 3

When working with SpecFlow 3, you can only configure your unit provider by adding the corresponding packages to your project. You will therefore need to add **one** of the following NuGet packages to your project to configure the unit test provider:

* SpecRun.SpecFlow-3.1.0
* SpecFlow.xUnit
* SpecFlow.MsTest
* SpecFlow.NUnit

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

### `<language>`

Use this section to define the default language for feature files and other language-related settings. For more details on language settings, see [Feature Language](../Gherkin/Feature-Language.md).

<table>
    <tr>
        <th>Attribute</th>
        <th>Value</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>feature</td>
        <td>culture name (“en-US”)</td>
        <td>The default language of feature files added to the project. We recommend using specific culture names (e.g.: “en-US”) rather than generic (neutral) cultures (e.g.: “en”). <br/>
            <b>Default:</b> en-US</td>
    </tr>
    <tr>
        <td>tool</td>
        <td>empty or culture name</td>
        <td>Specifies the language that SpecFlow uses for messages and tracing. Uses the default feature language if empty and that language is supported; otherwise messages are displayed in English. (<b>Note:</b> Only English is currently supported.)<br/>
            <b>Default:</b> empty</td>
    </tr>
</table>

### `<bindingCulture>`

Use this section to define the culture for executing binding methods and converting step arguments. For more details on language settings, see [Feature Language](../Gherkin/Feature-Language.md).

<table>
    <tr>
        <th>Attribute</th>
        <th>Value</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>name</td>
        <td>culture name (“en-US”)</td>
        <td>Specifies the culture to be used to execute binding methods and convert step arguments. If not specified, the feature language is used.<br/>
            <b>Default:</b> not specified</td>
    </tr>
</table>

### `<unitTestProvider>` (**Legacy option: Full Framework and app.config only!**)

**Note:** This option was removed with SpecFlow 3.0

Use this section to specify the unit test framework used by SpecFlow to execute acceptance criteria. You can either use one of the built-in unit test providers or you can specify the classes that implement custom unit test providers.

<table>
    <tr>
        <th>Attribute</th>
        <th>Value</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>name</td>
        <td>Name of the unit test provider. See [Unit Test Providers]().</td>
        <td>The name of the built-in unit test provider. If you specify this attribute, you don’t have to specify the other two.<br/><br />
            <b>Default:</b> nunit
<br /><br />

The following lists all supported providers with their name and generator provider class name:

<table>
 <tr>
  <th>Name</th>
  <th>Runtime Provider</th>
 </tr>
 <tr>
  <td>specrun</td>
  <td>SpecRunTestGeneratorProvider, see [the SpecFlow+ documentation|https://specflow.org/plus/documentation/]()</td>
 </tr>
 <tr>
  <td>nunit</td>
  <td>NUnit3TestGeneratorProvider</td>
 </tr>
 <tr>
  <td>nunit.2</td>
  <td>NUnit2TestGeneratorProvider</td>
 </tr>
 <tr>
  <td>xunit.1</td>
  <td>XUnitGeneratorProvider</td>
 </tr>
 <tr>
  <td>xunit</td>
  <td>XUnit2GeneratorProvider</td>
 </tr>
 <tr>
  <td>mstest.2008</td>
  <td>MsTestGeneratorProvider</td>
 </tr>
 <tr>
  <td>mstest</td>
  <td>MsTest2010GeneratorProvider</td>
 </tr>
 <tr>
  <td>mstest.v2</td>
  <td>MsTestV2GeneratorProvider</td>
 </tr>
</table>
</td>
    </tr>
    <tr>
        <td>generatorProvider</td>
        <td>class name</td>
        <td>Legacy option, unsupported from v2.0. Use [<code>&lt;plugins&gt;</code>|Plugins]() instead.</td>
    </tr>
    <tr>
        <td>runtimeProvider</td>
        <td>class name</td>
        <td>Legacy option, unsupported from v2.0. Use [<code>&lt;plugins&gt;</code>|Plugins]() instead.</td>
    </tr>
</table>



### `<generator>`

Use this section to define unit test generation options.

<table>
    <tr>
        <th>Attribute</th>
        <th>Value</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>allowDebugGeneratedFiles</td>
        <td>true|false</td>
        <td>By default, the debugger is configured to step through the generated code. This helps you debug your feature files and bindings (see [Debugging Tests]()). Disabled this option by setting this attribute to “true”.<br/>
            <b>Default:</b> false</td>
    </tr>
    <tr>
        <td>allowRowTests</td>
        <td>true|false</td>
        <td>Determines whether "row tests" should be generated for [scenario outlines|Using Gherkin Language in SpecFlow](). This setting is ignored if the [unit test framework|Unit Test Providers]() does not support row based testing.<br/>
            <b>Default:</b> true</td>
    </tr>
    <tr>
        <td>generateAsyncTests</td>
        <td>true|false</td>
        <td>Determines whether the generated tests should support [testing asynchronous code|Testing Silverlight Asynchronous Code](). This setting is currently only supported for the Silverlight platform.<br/>
            <b>Default:</b> false</td>
    </tr>
    <tr>
        <td>path</td>
        <td>path relative to the project folder</td>
        <td>Specifies the custom folder of the SpecFlow generator to be used if it is not in the standard path search list. See [Setup SpecFlow Projects]() for details.<br/>
            <b>Default:</b> not specified</td>
    </tr>
    <tr>
        <td>dependencies</td>
        <td>custom dependencies</td>
        <td>Specifies the custom dependencies for the SpecFlow generator. See [Plugins]() for details.<br/>
            <b>Default:</b> not specified</td>
    </tr>
</table>

### `<runtime>`

Use this section to specify various test execution options.

<table>
    <tr>
        <th>Attribute</th>
        <th>Value</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>dependencies</td>
        <td>custom dependencies</td>
        <td>Specifies custom dependencies for the SpecFlow runtime. See [Plugins]() for details.<br/>
            <b>Default:</b> not specified</td>
    </tr>
    <tr>
        <td>detectAmbiguousMatches</td>
        <td>true|false</td>
        <td>Legacy option, unsupported from v2.0.<br/>
            <b>Default:</b> true</td>
    </tr>
    <tr>
        <td>missingOrPendingStepsOutcome</td>
        <td>Inconclusive|<br/>Ignore|<br/>Error</td>
        <td>Determines how SpecFlow behaves if a step binding is not implemented or pending. See [Missing, Pending or Improperly Configured Bindings|Test Result]().<br/>
            <b>Default:</b> Inconclusive</td>
    </tr>
    <tr>
        <td>obsoleteBehavior</td>
        <td>None|Warn|<br/>Pending|Error</td>
        <td>Determines how SpecFlow behaves if a step binding is marked with [Obsolete] attribute.<br/>
            <b>Default:</b> Warn</td>
    </tr>
    <tr>
        <td>stopAtFirstError</td>
        <td>true|false</td>
        <td>Determines whether the execution should stop when encountering the first error, or whether it should attempt to try and match subsequent steps (in order to detect missing steps).<br/>
            <b>Default:</b> false</td>
    </tr>
</table>

### `<trace>`

Use this section to determine the SpecFlow trace output.

<table>
    <tr>
        <th>Attribute</th>
        <th>Value</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>traceSuccessfulSteps</td>
        <td>true|false</td>
        <td>Determines whether SpecFlow should trace successful step binding executions. <br/>
            <b>Default:</b> true</td>
    </tr>
    <tr>
        <td>traceTimings</td>
        <td>true|false</td>
        <td>Determines whether SpecFlow should trace execution time of the binding methods (only if the execution time is longer than the minTracedDuration value).<br/>
            <b>Default:</b> false</td>
    </tr>
    <tr>
        <td>minTracedDuration</td>
        <td>TimeSpan (0:0:0.1)</td>
        <td>Specifies a threshold for tracing the binding execution times.<br/>
            <b>Default:</b> 0:0:0.1 (100 ms)</td>
    </tr>
    <tr>
        <td>stepDefinitionSkeletonStyle</td>
        <td>RegexAttribute|<br/>MethodNameUnderscores|<br/>MethodNamePascalCase|<br/>MethodNameRegex</td>
        <td>Specifies the default [step definition style|Step Definition Styles]().<br/>
            <b>Default:</b> RegexAttribute</td>
    </tr>
    <tr>
        <td>Listener</td>
        <td>class name</td>
        <td>Legacy option, unsupported from v2.0. Use [<code>&lt;plugins&gt;</code>|Plugins]() instead.</td>
    </tr>
</table>

### `<stepAssemblies>`

This section can be used to configure additional assemblies that contain [external binding assemblies](../Bindings/Use-Bindings-from-External-Assemblies.md). The assembly of the SpecFlow project (the project containing the feature files) is automatically included. The binding assemblies must be placed in the output folder (e.g. bin/Debug) of the SpecFlow project, for example by adding a reference to the assembly from the project. 

The following example registers an additional binding assembly (MySharedBindings.dll). 

**app.config example:**
```xml
<specFlow>
  <stepAssemblies>
    <stepAssembly assembly="MySharedBindings" />
  </stepAssemblies>
</specFlow>
```


**specflow.json example:**
```
{
    "stepAssemblies": [
        { "assembly": "MySharedBindings" }
    ]
}
```

The `<stepAssemblies>` can contain multiple `<stepAssembly>` elements (one for each assembly), with the following attributes.

<table>
    <tr>
        <th>Attribute</th>
        <th>Value</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>assembly</td>
        <td>assembly name</td>
        <td>The name of the assembly containing bindings.</td>
    </tr>
</table>

#### `<plugins>`

This section can be used to configure plugins that contain customisations. See [Plugins](../Extend/Plugins.md) for more details.

```xml
<specFlow>
  <plugins>
    <add name="MyPlugin" />
  </plugins>
</specFlow>
```

The `<plugins>` element can contain multiple `<add>` elements (one for each plugin), with the following attributes:

<table>
    <tr>
        <th>Attribute</th>
        <th>Value</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>name</td>
        <td>plugin name</td>
        <td>The name of the plugin containing the customisations.</td>
    </tr>
    <tr>
        <td>path</td>
        <td>path relative to the project folder</td>
        <td>Specifies the custom folder of the SpecFlow plugin to be used if it is not in the standard path search list. See [Plugins]() for details.<br/>
            <b>Default:</b> not specified</td>
    </tr>
    <tr>
        <td>type</td>
        <td>Generator|<br/>Runtime|<br/>GeneratorAndRuntime</td>
        <td>Specifies whether the plugin customises the generator, the runtime or both.<br/>
            <b>Default:</b> GeneratorAndRuntime</td>
    </tr>
</table>