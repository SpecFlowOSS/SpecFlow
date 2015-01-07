using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Bindings
{
	public interface IRegexBinding : IBinding
	{
		Regex Regex { get; }
	}

	public interface IStepDefinitionBinding : IScopedBinding, IRegexBinding
	{
        StepDefinitionType StepDefinitionType { get; }
	}
}