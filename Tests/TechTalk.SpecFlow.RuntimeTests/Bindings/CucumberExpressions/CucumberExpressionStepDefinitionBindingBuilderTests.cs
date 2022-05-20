using FluentAssertions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.CucumberExpressions;
using TechTalk.SpecFlow.Bindings.Reflection;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings.CucumberExpressions;

public class CucumberExpressionStepDefinitionBindingBuilderTests
{
    private void SampleBindingMethod()
    {
        //nop
    }
    private CucumberExpressionStepDefinitionBindingBuilder CreateSut(string sourceExpression, StepDefinitionType stepDefinitionType = StepDefinitionType.Given, IBindingMethod bindingMethod = null, BindingScope bindingScope = null)
    {
        bindingMethod ??= new RuntimeBindingMethod(GetType().GetMethod(nameof(SampleBindingMethod)));
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

    //TODO[cukeex]: figure out what to expect
    //[Fact]
    //public void Should_build_from_expression_with_string_param()
    //{
    //    var sut = CreateSut("there is a user {string} registered");

    //    var result = sut.BuildSingle();

    //    result.ExpressionType.Should().Be(StepDefinitionExpressionTypes.CucumberExpression);
    //    result.Regex?.ToString().Should().Be(@"^there is a user () registered$");
    //}
}