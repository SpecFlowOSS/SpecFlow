using System;
using System.Text.RegularExpressions;
using CucumberExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings;

public class StepDefinitionBinding : MethodBinding, IStepDefinitionBinding
{
    public StepDefinitionType StepDefinitionType { get; }

    public string SourceExpression { get; }

    public string ExpressionType { get; }

    public bool IsValid => ErrorMessage == null;

    public string ErrorMessage { get; }

    public Regex Regex => Expression?.Regex;

    public IExpression Expression { get; }

    public BindingScope BindingScope { get; }
    public bool IsScoped => BindingScope != null;

    public StepDefinitionBinding(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string expressionType, string sourceExpression, IExpression expression)
        : base(bindingMethod)
    {
        StepDefinitionType = stepDefinitionType;
        BindingScope = bindingScope;
        ExpressionType = expressionType ?? throw new ArgumentNullException(nameof(expressionType));
        SourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        ErrorMessage = null;
    }

    private StepDefinitionBinding(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod, BindingScope bindingScope, string expressionType, string sourceExpression, string errorMessage)
        : base(bindingMethod)
    {
        StepDefinitionType = stepDefinitionType;
        Expression = null;
        BindingScope = bindingScope;
        SourceExpression = sourceExpression;
        ExpressionType = expressionType;
        ErrorMessage = errorMessage;
    }

    public static StepDefinitionBinding CreateInvalid(StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod,
        BindingScope bindingScope, string expressionType, string sourceExpression, string errorMessage)
        => new(stepDefinitionType, bindingMethod, bindingScope, expressionType, sourceExpression, errorMessage);
}