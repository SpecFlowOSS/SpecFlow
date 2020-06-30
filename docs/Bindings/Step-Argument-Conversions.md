# Step Argument Conversions

[Step bindings](Step-Definitions.md) can use parameters to make them reusable for similar steps. The parameters are taken from either the step's text or from the values in additional examples. These arguments are provided as either strings or `TechTalk.SpecFlow.Table` instances.

To avoid cumbersome conversions in the step binding methods, SpecFlow can perform an automatic conversion from the arguments to the parameter type in the binding method. All conversions are performed using the culture of the feature file, unless the [bindingCulture element](../Configuration/Configuration.md) is defined in your `app.config` file (see [Feature Language](../Gherkin/Feature-Language.md)). The following conversions can be performed by SpecFlow (in the following precedence):

* no conversion, if the argument is an instance of the parameter type (e.g. the parameter type is `object` or `string`)
* step argument transformation
* standard conversion

<!-- why is this not the order of the bulleted list above? -->

## Standard Conversion

A standard conversion is performed by SpecFlow in the following cases:

* The argument can be converted to the parameter type using `Convert.ChangeType()`
* The parameter type is an `enum` type and the (string) argument is an enum value
* The parameter type is `Guid` and the argument contains a full GUID string or a GUID string prefix. In the latter case, the value is filled with trailing zeroes.