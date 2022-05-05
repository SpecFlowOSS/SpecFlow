using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CucumberExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public abstract class StepDefinitionBindingBuilderBase : IStepDefinitionBindingBuilder
{
    protected StepDefinitionType _stepDefinitionType;
    protected IBindingMethod _bindingMethod;
    protected BindingScope _bindingScope;
    protected string _sourceExpression;

    protected StepDefinitionBindingBuilderBase(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string sourceExpression)
    {
        _stepDefinitionType = stepDefinitionType;
        _bindingMethod = bindingMethod;
        _bindingScope = bindingScope;
        _sourceExpression = sourceExpression;
    }

    protected abstract string GetRegexSource(out string expressionType);

    public virtual IEnumerable<IStepDefinitionBinding> Build()
    {
        yield return BuildSingle();
    }

    protected virtual RegularExpression CreateExpression(out string expressionType)
    {
        var regexSource = GetRegexSource(out expressionType);
        var regex = new Regex(regexSource, RegexOptions.CultureInvariant);
        var expression = new RegularExpression(regex);
        return expression;
    }

    public virtual IStepDefinitionBinding BuildSingle()
    {
        string expressionType = StepDefinitionExpressionTypes.Unknown;
        try
        {
            var expression = CreateExpression(out expressionType);
            return new StepDefinitionBinding(_stepDefinitionType, _bindingMethod, _bindingScope, expressionType, _sourceExpression, expression);
        }
        catch (Exception ex)
        {
            var errorMessage = ex.Message;
            return StepDefinitionBinding.CreateInvalid(_stepDefinitionType, _bindingMethod, _bindingScope, expressionType, _sourceExpression, errorMessage);
        }
    }
}
