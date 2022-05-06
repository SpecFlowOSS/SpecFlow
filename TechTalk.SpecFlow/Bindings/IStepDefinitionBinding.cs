using System.Text.RegularExpressions;
using CucumberExpressions;

namespace TechTalk.SpecFlow.Bindings;

public interface IStepDefinitionBinding : IScopedBinding, IBinding
{
    StepDefinitionType StepDefinitionType { get; }
    string SourceExpression { get; }
    string ExpressionType { get; }
    bool IsValid { get; }
    string ErrorMessage { get; }
    Regex Regex { get; }
    IExpression Expression { get; }
}