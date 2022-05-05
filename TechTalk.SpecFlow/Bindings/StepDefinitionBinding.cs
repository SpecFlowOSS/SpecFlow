using System;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public class StepDefinitionBinding : MethodBinding, IStepDefinitionBinding
{
    public StepDefinitionType StepDefinitionType { get; }

    public string SourceExpression { get; }

    public string ExpressionType { get; }

    public bool IsValid => ValidationErrorMessage == null;

    public string ValidationErrorMessage { get; }

    public Regex Regex { get; }

    public BindingScope BindingScope { get; }
    public bool IsScoped => BindingScope != null;

    public StepDefinitionBinding(StepDefinitionType stepDefinitionType, Regex regex, IBindingMethod bindingMethod, BindingScope bindingScope, string expressionType, string sourceExpression)
        : base(bindingMethod)
    {
        StepDefinitionType = stepDefinitionType;
        Regex = regex ?? throw new ArgumentNullException(nameof(regex));
        BindingScope = bindingScope;
        SourceExpression = sourceExpression;
        ExpressionType = expressionType;
        ValidationErrorMessage = null;
    }

    private StepDefinitionBinding(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string expressionType, string sourceExpression, string errorMessage)
        : base(bindingMethod)
    {
        StepDefinitionType = stepDefinitionType;
        Regex = null;
        BindingScope = bindingScope;
        SourceExpression = sourceExpression;
        ExpressionType = expressionType;
        ValidationErrorMessage = errorMessage;
    }

    public static StepDefinitionBinding CreateInvalid(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod,
        BindingScope bindingScope, string expressionType, string sourceExpression, string errorMessage)
        => new(stepDefinitionType, bindingMethod, bindingScope, expressionType, sourceExpression, errorMessage);
}