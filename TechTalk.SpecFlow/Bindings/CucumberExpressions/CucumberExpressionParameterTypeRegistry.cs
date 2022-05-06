using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CucumberExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings.CucumberExpressions;

public class CucumberExpressionParameterTypeRegistry : IParameterTypeRegistry
{
    private readonly IBindingRegistry _bindingRegistry;
    private readonly Lazy<Dictionary<string, ISpecFlowCucumberExpressionParameterType>> _parameterTypesByName;

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
            // official cucumber expression types
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, objectBindingType, name: string.Empty, useForSnippets: false),
            new BuiltInCucumberExpressionParameterTypeTransformation(ParameterTypeConstants.IntParameterRegex, intBindingType, ParameterTypeConstants.IntParameterName, weight: 1000),
            new BuiltInCucumberExpressionParameterTypeTransformation(ParameterTypeConstants.FloatParameterRegex, doubleBindingType, ParameterTypeConstants.FloatParameterName),
            new BuiltInCucumberExpressionParameterTypeTransformation(ParameterTypeConstants.WordParameterRegex, stringBindingType, ParameterTypeConstants.WordParameterName, useForSnippets: false),

            // other types supported by SpecFlow by default: Make them accessible with type name (e.g. Int32)
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, boolBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, byteBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, charBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, dateTimeBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, decimalBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, doubleBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, floatBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, shortBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, intBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, longBindingType),
            new BuiltInCucumberExpressionParameterTypeTransformation(CucumberExpressionParameterType.MatchAllRegex, guidBindingType),
        };

        var convertQuotedStringMethod = new RuntimeBindingMethod(GetType().GetMethod(nameof(ConvertQuotedString)));
        _bindingRegistry.RegisterStepArgumentTransformationBinding(new CucumberExpressionParameterTypeBinding(ParameterTypeConstants.StringParameterRegexDoubleQuote, convertQuotedStringMethod, ParameterTypeConstants.StringParameterName));
        _bindingRegistry.RegisterStepArgumentTransformationBinding(new CucumberExpressionParameterTypeBinding(ParameterTypeConstants.StringParameterRegexApostrophe, convertQuotedStringMethod, ParameterTypeConstants.StringParameterName));

        var userTransformations = _bindingRegistry.GetStepTransformations().Select(t => new UserDefinedCucumberExpressionParameterTypeTransformation(t));

        var parameterTypes = builtInTransformations.Cast<ICucumberExpressionParameterTypeTransformation>()
                                                   .Concat(userTransformations)
                                                   .GroupBy(t => new Tuple<IBindingType, string>(t.TargetType, t.Name))
                                                   .Select(g => new CucumberExpressionParameterType(g.Key.Item2 ?? g.Key.Item1.Name, g.Key.Item1, g))
                                                   .ToDictionary(pt => pt.Name, pt => (ISpecFlowCucumberExpressionParameterType)pt);

        DumpParameterTypes(parameterTypes);

        return parameterTypes;
    }

    [Conditional("DEBUG")]
    private static void DumpParameterTypes(Dictionary<string, ISpecFlowCucumberExpressionParameterType> parameterTypes)
    {
        foreach (var parameterType in parameterTypes)
        {
            Console.WriteLine(
                $"PT: {parameterType.Key}, transformations: {string.Join(",", parameterType.Value.Transformations.Select(t => t.Regex))}, Regexps: {string.Join(",", parameterType.Value.RegexStrings)}");
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
}
