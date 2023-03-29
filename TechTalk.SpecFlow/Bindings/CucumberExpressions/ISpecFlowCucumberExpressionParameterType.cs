using System;
using CucumberExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.Bindings.CucumberExpressions;

public interface ISpecFlowCucumberExpressionParameterType : IParameterType
{
    IBindingType TargetType { get; }
    ICucumberExpressionParameterTypeTransformation[] Transformations { get; }
}