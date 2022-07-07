using System;
using System.Reflection;
using FluentAssertions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.CucumberExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings.CucumberExpressions;

public class CucumberExpressionStepDefinitionBindingBuilderTests
{
    // The different parameter types are tested in integration of a cucumber expression match,
    // see CucumberExpressionIntegrationTests class.

    private void SampleBindingMethod()
    {
        //nop
    }

    private CucumberExpressionStepDefinitionBindingBuilder CreateSut(string sourceExpression, StepDefinitionType stepDefinitionType = StepDefinitionType.Given, IBindingMethod bindingMethod = null, BindingScope bindingScope = null)
    {
        bindingMethod ??= new RuntimeBindingMethod(GetType().GetMethod(nameof(SampleBindingMethod), BindingFlags.NonPublic | BindingFlags.Instance));
        return new CucumberExpressionStepDefinitionBindingBuilder(
            new CucumberExpressionParameterTypeRegistry(new BindingRegistry()),
            stepDefinitionType,
            bindingMethod,
            bindingScope,
            sourceExpression);
    }

    [Fact]
    public void Should_build_from_simple_expression()
    {
        var sut = CreateSut("simple expression");

        var result = sut.BuildSingle();

        result.ExpressionType.Should().Be(StepDefinitionExpressionTypes.CucumberExpression);
        result.Regex?.ToString().Should().Be("^simple expression$");
    }

    [Fact]
    public void Should_build_from_expression_with_int_param()
    {
        var sut = CreateSut("I have {int} cucumbers in my belly");

        var result = sut.BuildSingle();

        result.ExpressionType.Should().Be(StepDefinitionExpressionTypes.CucumberExpression);
        result.Regex?.ToString().Should().Be(@"^I have (-?\d+) cucumbers in my belly$");
    }

    [Fact]
    public void Should_build_from_expression_with_DateTime_param()
    {
        var sut = CreateSut("I have eaten cucumbers on {DateTime}");

        var result = sut.BuildSingle();

        result.ExpressionType.Should().Be(StepDefinitionExpressionTypes.CucumberExpression);
        result.Regex?.ToString().Should().Be(@"^I have eaten cucumbers on (.*)$");
    }

    [Fact]
    public void Should_build_from_expression_with_string_param()
    {
        var sut = CreateSut("there is a user {string} registered");

        var result = sut.BuildSingle();

        result.ExpressionType.Should().Be(StepDefinitionExpressionTypes.CucumberExpression);
        result.Regex?.ToString().Should().Be(@"^there is a user (?:(?:""([^""\\]*(?:\\.[^""\\]*)*)"")|(?:'([^'\\]*(?:\\.[^'\\]*)*)')) registered$");
    }

    [Obsolete("this is deprecated", false)]
    // ReSharper disable once UnusedMember.Local
    private void SampleBindingMethod_Obsolete()
    {
        //nop
    }

    [Fact]
    public void Should_create_binding_that_can_be_detected_as_obsolete()
    {
        var runtimeBindingMethod = new RuntimeBindingMethod(GetType().GetMethod(nameof(SampleBindingMethod) + "_Obsolete", BindingFlags.NonPublic | BindingFlags.Instance));
        var sut = CreateSut("simple expression", bindingMethod: 
                            runtimeBindingMethod);

        var result = sut.BuildSingle();

        result.ExpressionType.Should().Be(StepDefinitionExpressionTypes.CucumberExpression);
        result.Regex?.ToString().Should().Be("^simple expression$");

        var obsoletion = new BindingObsoletion(result);
        obsoletion.IsObsolete.Should().BeTrue();
    }
}