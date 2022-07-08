using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings.CucumberExpressions;

public interface ICucumberExpressionParameterTypeTransformation
{
    string Name { get; }
    string Regex { get; }
    IBindingType TargetType { get; }
    bool UseForSnippets { get; }
    int Weight { get; }
}
