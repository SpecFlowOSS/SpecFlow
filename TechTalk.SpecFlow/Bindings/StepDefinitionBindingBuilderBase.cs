using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
