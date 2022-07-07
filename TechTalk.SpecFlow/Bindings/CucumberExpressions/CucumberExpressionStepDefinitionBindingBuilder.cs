using System;
using System.Text.RegularExpressions;
using CucumberExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings.CucumberExpressions;

public class CucumberExpressionStepDefinitionBindingBuilder : StepDefinitionBindingBuilderBase
{
    private static readonly Regex ParameterPlaceholder = new(@"{\w*}");
    private static readonly Regex CommonRegexStepDefPatterns = new(@"(\([^\)]+[\*\+]\)|\.\*)");
    private static readonly Regex ExtendedRegexStepDefPatterns = new(@"(\\\.|\\d\+)"); // \. \d+

    private readonly CucumberExpressionParameterTypeRegistry _cucumberExpressionParameterTypeRegistry;

    public CucumberExpressionStepDefinitionBindingBuilder(
        CucumberExpressionParameterTypeRegistry cucumberExpressionParameterTypeRegistry,
        StepDefinitionType stepDefinitionType,
        IBindingMethod bindingMethod,
        BindingScope bindingScope,
        string sourceExpression) : base(stepDefinitionType, bindingMethod, bindingScope, sourceExpression)
    {
        _cucumberExpressionParameterTypeRegistry = cucumberExpressionParameterTypeRegistry;
    }

    public static bool IsCucumberExpression(string cucumberExpressionCandidate)
    {
        if (cucumberExpressionCandidate.StartsWith("^") || cucumberExpressionCandidate.EndsWith("$"))
            return false;

        if (ParameterPlaceholder.IsMatch(cucumberExpressionCandidate))
            return true;

        if (CommonRegexStepDefPatterns.IsMatch(cucumberExpressionCandidate))
            return false;

        // These are special constructs that usually happen in regex, but not valid
        // in Cucumber Expressions => If they exist, we treat the expression as regex.
        // - \d+
        // - \.
        if (ExtendedRegexStepDefPatterns.IsMatch(cucumberExpressionCandidate))
            return false;

        return true;
    }

    protected override IExpression CreateExpression(out string expressionType)
    {
        expressionType = StepDefinitionExpressionTypes.CucumberExpression;
        return new SpecFlowCucumberExpression(_sourceExpression, _cucumberExpressionParameterTypeRegistry);
    }
}