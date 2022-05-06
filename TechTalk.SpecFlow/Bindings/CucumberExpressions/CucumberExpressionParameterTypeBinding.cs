using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings.CucumberExpressions;

public class CucumberExpressionParameterTypeBinding : StepArgumentTransformationBinding
{
    public string Name { get; }

    public CucumberExpressionParameterTypeBinding(string regexString, IBindingMethod bindingMethod, string name) : base(regexString, bindingMethod)
    {
        Name = name;
    }
}