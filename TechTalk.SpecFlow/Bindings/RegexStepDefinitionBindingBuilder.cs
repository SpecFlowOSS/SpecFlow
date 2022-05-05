using System.Collections.Generic;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public class RegexStepDefinitionBindingBuilder : IStepDefinitionBindingBuilder
{
    private readonly StepDefinitionType _stepDefinitionType;
    private readonly IBindingMethod _bindingMethod;
    private readonly BindingScope _bindingScope;
    private readonly string _sourceExpression;

    public RegexStepDefinitionBindingBuilder(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string sourceExpression)
    {
        _stepDefinitionType = stepDefinitionType;
        _bindingMethod = bindingMethod;
        _bindingScope = bindingScope;
        _sourceExpression = sourceExpression;
    }

    protected virtual string GetRegexSource()
    {
        var regex = _sourceExpression;
        if (!regex.StartsWith("^")) regex = "^" + regex;
        if (!regex.EndsWith("$")) regex += "$";
        return regex;
    }

    public IEnumerable<IStepDefinitionBinding> Build()
    {
        //TODO[cukeex]: error handling
        var regexSource = GetRegexSource();
        var regex = new Regex(regexSource, RegexOptions.CultureInvariant);
        yield return new StepDefinitionBinding(_stepDefinitionType, regex, _bindingMethod, _bindingScope);
    }
}
