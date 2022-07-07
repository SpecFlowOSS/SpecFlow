using System;
using System.Collections.Generic;
using System.Linq;
using CucumberExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings.CucumberExpressions;

public class CucumberExpressionParameterTypeRegistry : IParameterTypeRegistry
{
    private readonly IBindingRegistry _bindingRegistry;
    private readonly Lazy<Dictionary<string, ISpecFlowCucumberExpressionParameterType>> _parameterTypesByName;
    private readonly HashSet<IBindingType> _consideredParameterTypes = new();

    public CucumberExpressionParameterTypeRegistry(IBindingRegistry bindingRegistry)
    {
        _bindingRegistry = bindingRegistry;
        _parameterTypesByName = new Lazy<Dictionary<string, ISpecFlowCucumberExpressionParameterType>>(InitializeRegistry, true);
    }

    public static string ConvertQuotedString(string message)
    {
        return message;
    }

    private Dictionary<string, ISpecFlowCucumberExpressionParameterType> InitializeRegistry()
    {
        var boolBindingType = new RuntimeBindingType(typeof(bool));
        var byteBindingType = new RuntimeBindingType(typeof(byte));
        var charBindingType = new RuntimeBindingType(typeof(char));
        var dateTimeBindingType = new RuntimeBindingType(typeof(DateTime));
        var decimalBindingType = new RuntimeBindingType(typeof(decimal));
        var doubleBindingType = new RuntimeBindingType(typeof(double));
        var floatBindingType = new RuntimeBindingType(typeof(float));
        var shortBindingType = new RuntimeBindingType(typeof(short));
        var intBindingType = new RuntimeBindingType(typeof(int));
        var longBindingType = new RuntimeBindingType(typeof(long));
        var objectBindingType = new RuntimeBindingType(typeof(object));
        var stringBindingType = new RuntimeBindingType(typeof(string));
        var guidBindingType = new RuntimeBindingType(typeof(Guid));

        // exposing built-in transformations

        var builtInTransformations = new[]
        {
            // official cucumber expression special expression parameter types ({}, {word})
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, objectBindingType, name: string.Empty, useForSnippets: false),
            new BuiltInCucumberExpressionParameterTypeTransformation(ParameterTypeConstants.WordParameterRegex, stringBindingType, ParameterTypeConstants.WordParameterName, useForSnippets: false),

            // official cucumber expression string expression parameter type ({string})
            // The regex '.*' specified here will be ignored because of the special string type handling implemented in SpecFlowCucumberExpression.
            // See SpecFlowCucumberExpression.HandleStringType for detailed explanation.
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, stringBindingType, ParameterTypeConstants.StringParameterName),

            // other types supported by SpecFlow by default: Make them accessible with type name (e.g. {Guid})
            new BuiltInCucumberExpressionParameterTypeTransformation(ParameterTypeConstants.IntParameterRegex, byteBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(ParameterTypeConstants.IntParameterRegex, shortBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(ParameterTypeConstants.IntParameterRegex, intBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(ParameterTypeConstants.IntParameterRegex, longBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(ParameterTypeConstants.FloatParameterRegex, doubleBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(ParameterTypeConstants.FloatParameterRegex, floatBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(ParameterTypeConstants.FloatParameterRegex, decimalBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, boolBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, charBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, dateTimeBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, guidBindingType),
        };

        // cucumber expression aliases for supported types ({int}, {float}, {double}, {decimal})
        var aliases = new Dictionary<IBindingType, string>
        {
            { intBindingType, "int" }, // official
            { floatBindingType, "float" }, // official
            { doubleBindingType, "double" }, // official
            { byteBindingType, "byte" }, // official
            { longBindingType, "long" }, // official
            { decimalBindingType, "decimal" } // custom for .NET
        };

        var aliasTransformations = builtInTransformations.Select(t => aliases.TryGetValue(t.TargetType, out var alias) ? new { Transformation = t, Alias = alias } : null)
                                                         .Where(t => t != null)
                                                         .Select(t => new BuiltInCucumberExpressionParameterTypeTransformation(t.Transformation.Regex, t.Transformation.TargetType, t.Alias))
                                                         .ToArray();

        // cucumber expression parameter type support for enums with type name, using the built-in enum transformation (e.g. {Color})
        var enumTypes = GetEnumTypesUsedInParameters();

        // get custom user transformations (both for built-in types and for custom types)
        var userTransformations = _bindingRegistry.GetStepTransformations()
                                                  .SelectMany(t => GetUserTransformations(t, aliases));

        var parameterTypes = builtInTransformations
                                                   .Concat(enumTypes)
                                                   .Concat(aliasTransformations)
                                                   .Concat(userTransformations)
                                                   .GroupBy(t => (t.TargetType, t.Name))
                                                   .Select(g => new CucumberExpressionParameterType(g.Key.Name ?? g.Key.TargetType.Name, g.Key.TargetType, g))
                                                   .ToDictionary(pt => pt.Name, pt => (ISpecFlowCucumberExpressionParameterType)pt);

        return parameterTypes;
    }

    private IEnumerable<UserDefinedCucumberExpressionParameterTypeTransformation> GetUserTransformations(IStepArgumentTransformationBinding t, Dictionary<IBindingType, string> aliases)
    {
        yield return new UserDefinedCucumberExpressionParameterTypeTransformation(t);

        // If the custom user transformations is for a built-in type, we also expose it with the
        // short name (e.g {int}) and not only with the type name (e.g. {Int32}).
        if (aliases.TryGetValue(t.Method.ReturnType, out var alias))
            yield return new UserDefinedCucumberExpressionParameterTypeTransformation(t, alias);
    }

    private IEnumerable<ICucumberExpressionParameterTypeTransformation> GetEnumTypesUsedInParameters()
    {
        // As we don't have a full list of possible enums, we collect all enums that are used as parameters of the step definitions.
        var enumParameterTypes = _consideredParameterTypes
                                 .OfType<RuntimeBindingType>()
                                 .Where(t => t.Type.IsEnum);
        foreach (var enumParameterType in enumParameterTypes)
        {
            yield return new BuiltInCucumberExpressionParameterTypeTransformation(
                CucumberExpressionParameterType.MatchAllRegex,
                enumParameterType);
        }
    }

    public IParameterType LookupByTypeName(string name)
    {
        if (_parameterTypesByName.Value.TryGetValue(name, out var parameterType))
            return parameterType;
        return null;
    }

    public IEnumerable<IParameterType> GetParameterTypes()
    {
        return _parameterTypesByName.Value.Values;
    }

    public void OnBindingMethodProcessed(IBindingMethod bindingMethod)
    {
        var parameterTypes = bindingMethod.Parameters
                             .Select(p => p.Type);
        foreach (var parameterType in parameterTypes)
        {
            _consideredParameterTypes.Add(parameterType);
        }
    }
}
