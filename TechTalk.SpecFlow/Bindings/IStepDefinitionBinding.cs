using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings;

public interface IStepDefinitionBinding : IScopedBinding, IBinding
{
    StepDefinitionType StepDefinitionType { get; }
    string SourceExpression { get; }
    string ExpressionType { get; }
    bool IsValid { get; }
    string ValidationErrorMessage { get; }
    Regex Regex { get; }
}