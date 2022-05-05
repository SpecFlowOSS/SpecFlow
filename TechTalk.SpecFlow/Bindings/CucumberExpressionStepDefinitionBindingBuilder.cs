using System;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public class CucumberExpressionStepDefinitionBindingBuilder : StepDefinitionBindingBuilderBase
{
    private static readonly Regex ParameterPlaceholder = new(@"{\w*}");
    private static readonly Regex CommonRegexStepDefPatterns = new(@"(\([^\)]+[\*\+]\)|\.\*)");

    public CucumberExpressionStepDefinitionBindingBuilder(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string sourceExpression) : base(stepDefinitionType, bindingMethod, bindingScope, sourceExpression)
    {
    }

    public static bool IsCucumberExpression(string cucumberExpressionCandidate)
    {
        if (cucumberExpressionCandidate.StartsWith("^") || cucumberExpressionCandidate.EndsWith("$"))
            return false;

        if (ParameterPlaceholder.IsMatch(cucumberExpressionCandidate))
            return true;

        if (CommonRegexStepDefPatterns.IsMatch(cucumberExpressionCandidate))
            return false;

        return true;
    }

    protected override string GetRegexSource(out string expressionType)
    {
        expressionType = StepDefinitionExpressionTypes.CucumberExpression;
        if (!ParameterPlaceholder.IsMatch(_sourceExpression))
            return "^" + _sourceExpression + "$";

        throw new NotImplementedException();
    }
}
