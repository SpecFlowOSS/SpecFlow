using System;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public class RegexStepDefinitionBindingBuilder : StepDefinitionBindingBuilderBase
{
    public RegexStepDefinitionBindingBuilder(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string sourceExpression) : base(stepDefinitionType, bindingMethod, bindingScope, sourceExpression)
    {
    }

    protected override string GetRegexSource(out string expressionType)
    {
        expressionType = StepDefinitionExpressionTypes.RegularExpression;
        var regex = _sourceExpression;
        if (!regex.StartsWith("^")) regex = "^" + regex;
        if (!regex.EndsWith("$")) regex += "$";
        return regex;
    }
}