using System;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.RuntimeTests;

internal static class StepDefinitionHelper
{
    public static IStepDefinitionBinding CreateRegex(StepDefinitionType stepDefinitionType, string regex, IBindingMethod bindingMethod = null, BindingScope bindingScope = null)
    {
        bindingMethod ??= new Mock<IBindingMethod>().Object;
        var builder = new RegexStepDefinitionBindingBuilder(stepDefinitionType, bindingMethod, bindingScope, regex);
        var stepDefinitionBinding = builder.BuildSingle();
        stepDefinitionBinding.IsValid.Should().BeTrue($"the {nameof(CreateRegex)} method should create valid step definitions");
        return stepDefinitionBinding;
    }
}