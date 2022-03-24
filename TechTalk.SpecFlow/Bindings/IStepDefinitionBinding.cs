using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IStepDefinitionBinding : IScopedBinding, IBinding
    {
        StepDefinitionType StepDefinitionType { get; }
        Regex Regex { get; }
    }
}