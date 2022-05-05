using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public class RegexStepDefinitionBindingBuilder : IStepDefinitionBindingBuilder
{
    protected readonly StepDefinitionType _stepDefinitionType;
    protected readonly IBindingMethod _bindingMethod;
    protected readonly BindingScope _bindingScope;
    protected readonly string _sourceExpression;

    public RegexStepDefinitionBindingBuilder(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string sourceExpression)
    {
        _stepDefinitionType = stepDefinitionType;
        _bindingMethod = bindingMethod;
        _bindingScope = bindingScope;
        _sourceExpression = sourceExpression;
    }

    protected virtual string GetRegexSource(out string expressionType)
    {
        expressionType = StepDefinitionExpressionTypes.RegularExpression;
        var regex = _sourceExpression;
        if (!regex.StartsWith("^")) regex = "^" + regex;
        if (!regex.EndsWith("$")) regex += "$";
        return regex;
    }

    public virtual IEnumerable<IStepDefinitionBinding> Build()
    {
        yield return BuildSingle();
    }

    public virtual IStepDefinitionBinding BuildSingle()
    {
        string expressionType = StepDefinitionExpressionTypes.Unknown;
        try
        {
            var regexSource = GetRegexSource(out expressionType);
            var regex = new Regex(regexSource, RegexOptions.CultureInvariant);
            return new StepDefinitionBinding(_stepDefinitionType, regex, _bindingMethod, _bindingScope, expressionType, _sourceExpression);
        }
        catch (Exception ex)
        {
            var errorMessage = ex.Message;
            return StepDefinitionBinding.CreateInvalid(_stepDefinitionType, _bindingMethod, _bindingScope, expressionType, _sourceExpression, errorMessage);
        }
    }
}
