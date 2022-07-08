using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings.CucumberExpressions;

public class CucumberExpressionParameterType : ISpecFlowCucumberExpressionParameterType
{
    internal const string MatchAllRegex = @".*";

    public string Name { get; }
    public IBindingType TargetType { get; }
    public ICucumberExpressionParameterTypeTransformation[] Transformations { get; }
    public string[] RegexStrings { get; }

    public bool UseForSnippets { get; }
    public int Weight { get; }

    public Type ParameterType => ((RuntimeBindingType)TargetType).Type;

    public CucumberExpressionParameterType(string name, IBindingType targetType, IEnumerable<ICucumberExpressionParameterTypeTransformation> transformations)
    {
        Name = name;
        TargetType = targetType;
        Transformations = transformations.ToArray();
        UseForSnippets = Transformations.Any(t => t.UseForSnippets);
        Weight = Transformations.Max(t => t.Weight);

        var regexStrings = Transformations.Select(tr => tr.Regex).Distinct().ToArray();
        if (regexStrings.Length > 1 && regexStrings.Contains(MatchAllRegex))
            regexStrings = new[] {MatchAllRegex};
        RegexStrings = regexStrings;
    }
}