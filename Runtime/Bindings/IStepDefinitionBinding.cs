using System;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

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