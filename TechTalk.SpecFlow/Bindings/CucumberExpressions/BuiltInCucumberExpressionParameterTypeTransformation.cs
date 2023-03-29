using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings.CucumberExpressions;

public class BuiltInCucumberExpressionParameterTypeTransformation : ICucumberExpressionParameterTypeTransformation
{
    public string Name { get; }
    public string Regex { get; }
    public IBindingType TargetType { get; }
    public bool UseForSnippets { get; }
    public int Weight { get; }

    public BuiltInCucumberExpressionParameterTypeTransformation(string regex, IBindingType targetType, string name = null, bool useForSnippets = true, int weight = 0)
    {
        Regex = regex;
        TargetType = targetType;
        Name = name;
        UseForSnippets = useForSnippets;
        Weight = weight;
    }
}