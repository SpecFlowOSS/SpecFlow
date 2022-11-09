using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public class InvalidStepDefinitionBindingBuilder : IStepDefinitionBindingBuilder
{
    private readonly StepDefinitionType _stepDefinitionType;
    private readonly IBindingMethod _bindingMethod;
    private readonly BindingScope _bindingScope;
    private readonly string _sourceExpression;
    private readonly string _errorMessage;

    public InvalidStepDefinitionBindingBuilder(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string sourceExpression, string errorMessage)
    {
        _stepDefinitionType = stepDefinitionType;
        _bindingMethod = bindingMethod;
        _bindingScope = bindingScope;
        _sourceExpression = sourceExpression;
        _errorMessage = errorMessage;
    }

    public IEnumerable<IStepDefinitionBinding> Build()
    {
        yield return StepDefinitionBinding.CreateInvalid(_stepDefinitionType, _bindingMethod, _bindingScope, StepDefinitionExpressionTypes.Unknown, _sourceExpression, _errorMessage);
    }
}
