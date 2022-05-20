using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using BoDi;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings.CucumberExpressions;

public enum SampleColorEnum
{
    Yellow,
    Brown
}

public class CucumberExpressionIntegrationTests
{
    public class SampleBindings
    {
        public bool WasStepDefWithNoParamExecuted;
        public List<(object ParamValue, Type ParamType)> ExecutedParams = new();

        public void StepDefWithNoParam()
        {
            WasStepDefWithNoParamExecuted = true;
        }

        public void StepDefWithStringParam(string stringParam)
        {
            ExecutedParams.Add((stringParam, typeof(string)));
        }

        public void StepDefWithIntParam(int intParam)
        {
            ExecutedParams.Add((intParam, typeof(int)));
        }
        
        public void StepDefWithDoubleParam(double doubleParam)
        {
            ExecutedParams.Add((doubleParam, typeof(double)));
        }
        public void StepDefWithDecimalParam(decimal decimalParam)
        {
            ExecutedParams.Add((decimalParam, typeof(decimal)));
        }
        public void StepDefWithEnumParam(SampleColorEnum enumParam)
        {
            ExecutedParams.Add((enumParam, typeof(SampleColorEnum)));
        }
    }

    public class TestDependencyProvider : DefaultDependencyProvider
    {
        public override void RegisterGlobalContainerDefaults(ObjectContainer container)
        {
            base.RegisterGlobalContainerDefaults(container);
            var stubUintTestProvider = new Mock<IUnitTestRuntimeProvider>();
            container.RegisterInstanceAs(stubUintTestProvider.Object, "nunit");
        }
    }

    private async Task<SampleBindings> PerformStepExecution(string methodName, string expression, string stepText)
    {
        var containerBuilder = new ContainerBuilder(new TestDependencyProvider());
        var globalContainer = containerBuilder.CreateGlobalContainer(GetType().Assembly);
        var testThreadContainer = containerBuilder.CreateTestThreadContainer(globalContainer);
        var engine = testThreadContainer.Resolve<ITestExecutionEngine>();

        var bindingSourceProcessor = globalContainer.Resolve<IRuntimeBindingSourceProcessor>();
        var bindingSourceMethod = new BindingSourceMethod
        {
            BindingMethod = new RuntimeBindingMethod(typeof(SampleBindings).GetMethod(methodName)),
            IsPublic = true,
            Attributes = new[]
            {
                new BindingSourceAttribute
                {
                    AttributeType = new RuntimeBindingType(typeof(GivenAttribute)),
                    AttributeValues = new IBindingSourceAttributeValueProvider[]
                    {
                        new BindingSourceAttributeValueProvider(expression)
                    }
                }
            }
        };
        bindingSourceProcessor.ProcessType(
            new BindingSourceType
            {
                BindingType = new RuntimeBindingType(typeof(SampleBindings)),
                Attributes = new[]
                {
                    new BindingSourceAttribute
                    {
                        AttributeType = new RuntimeBindingType(typeof(BindingAttribute))
                    }
                },
                IsPublic = true,
                IsClass = true
            });
        bindingSourceProcessor.ProcessMethod(bindingSourceMethod);
        bindingSourceProcessor.BuildingCompleted();

        await engine.OnTestRunStartAsync();
        await engine.OnFeatureStartAsync(new FeatureInfo(CultureInfo.GetCultureInfo("en-US"), ".", "Sample feature", null, ProgrammingLanguage.CSharp));
        await engine.OnScenarioStartAsync();
        engine.OnScenarioInitialize(new ScenarioInfo("Sample scenario", null, null, null));
        await engine.StepAsync(StepDefinitionKeyword.Given, "Given ", stepText, null, null);

        var contextManager = testThreadContainer.Resolve<IContextManager>();
        contextManager.ScenarioContext.ScenarioExecutionStatus.Should().Be(ScenarioExecutionStatus.OK, $"should not fail with '{contextManager.ScenarioContext.TestError?.Message}'");

        return contextManager.ScenarioContext.ScenarioContainer.Resolve<SampleBindings>();
    }

    [Fact]
    public async void Should_match_step_with_simple_cucumber_expression()
    {
        var expression = "there is something";
        var stepText = "there is something";
        var methodName = nameof(SampleBindings.StepDefWithNoParam);

        var sampleBindings = await PerformStepExecution(methodName, expression, stepText);

        sampleBindings.WasStepDefWithNoParamExecuted.Should().BeTrue();
    }

    [Fact]
    public async void Should_match_step_with_parameterless_cucumber_expression()
    {
        var expression = "there is/are something(s) here \\/ now";
        var stepText = "there are something here / now";
        var methodName = nameof(SampleBindings.StepDefWithNoParam);

        var sampleBindings = await PerformStepExecution(methodName, expression, stepText);

        sampleBindings.WasStepDefWithNoParamExecuted.Should().BeTrue();
    }

    [Fact]
    public async void Should_match_step_with_string_parameter_using_apostrophe()
    {
        var expression = "there is a user {string} registered";
        var stepText = "there is a user 'Marvin' registered";
        var expectedParam = ("Marvin", typeof(string));
        var methodName = nameof(SampleBindings.StepDefWithStringParam);

        var sampleBindings = await PerformStepExecution(methodName, expression, stepText);

        sampleBindings.ExecutedParams.Should().Contain(expectedParam);
    }

    [Fact]
    public async void Should_match_step_with_string_parameter_using_quotes()
    {
        var expression = "there is a user {string} registered";
        var stepText = "there is a user \"Marvin\" registered";
        var expectedParam = ("Marvin", typeof(string));
        var methodName = nameof(SampleBindings.StepDefWithStringParam);

        var sampleBindings = await PerformStepExecution(methodName, expression, stepText);

        sampleBindings.ExecutedParams.Should().Contain(expectedParam);
    }

    [Fact]
    public async void Should_match_step_with_word_parameter()
    {
        var expression = "there is a user {word} registered";
        var stepText = "there is a user Marvin registered";
        var expectedParam = ("Marvin", typeof(string));
        var methodName = nameof(SampleBindings.StepDefWithStringParam);

        var sampleBindings = await PerformStepExecution(methodName, expression, stepText);

        sampleBindings.ExecutedParams.Should().Contain(expectedParam);
    }

    [Fact]
    public async void Should_match_step_with_int_parameter()
    {
        var expression = "I have {int} cucumbers in my belly";
        var stepText = "I have 42 cucumbers in my belly";
        var expectedParam = (42, typeof(int));
        var methodName = nameof(SampleBindings.StepDefWithIntParam);

        var sampleBindings = await PerformStepExecution(methodName, expression, stepText);

        sampleBindings.ExecutedParams.Should().Contain(expectedParam);
    }

    [Fact]
    public async void Should_match_step_with_float_parameter_to_double()
    {
        var expression = "I have {float} cucumbers in my belly";
        var stepText = "I have 42.1 cucumbers in my belly";
        var expectedParam = (42.1, typeof(double));
        var methodName = nameof(SampleBindings.StepDefWithDoubleParam);

        var sampleBindings = await PerformStepExecution(methodName, expression, stepText);

        sampleBindings.ExecutedParams.Should().Contain(expectedParam);
    }

    [Fact]
    public async void Should_match_step_with_float_parameter_to_decimal()
    {
        var expression = "I have {float} cucumbers in my belly";
        var stepText = "I have 42.1 cucumbers in my belly";
        var expectedParam = (42.1m, typeof(decimal));
        var methodName = nameof(SampleBindings.StepDefWithDecimalParam);

        var sampleBindings = await PerformStepExecution(methodName, expression, stepText);

        sampleBindings.ExecutedParams.Should().Contain(expectedParam);
    }

    [Fact]
    public async void Should_match_step_with_joker_parameter()
    {
        var expression = "there is a user {} registered";
        var stepText = "there is a user Marvin registered";
        var expectedParam = ("Marvin", typeof(string));
        var methodName = nameof(SampleBindings.StepDefWithStringParam);

        var sampleBindings = await PerformStepExecution(methodName, expression, stepText);

        sampleBindings.ExecutedParams.Should().Contain(expectedParam);
    }

    // build-in types supported by SpecFlow
    [Fact]
    public async void Should_match_step_with_Int32_parameter()
    {
        var expression = "I have {Int32} cucumbers in my belly";
        var stepText = "I have 42 cucumbers in my belly";
        var expectedParam = (42, typeof(int));
        var methodName = nameof(SampleBindings.StepDefWithIntParam);

        var sampleBindings = await PerformStepExecution(methodName, expression, stepText);

        sampleBindings.ExecutedParams.Should().Contain(expectedParam);
    }

    // enum support
    [Fact]
    public async void Should_match_step_with_enum_parameter()
    {
        var expression = "I have {SampleColorEnum} cucumbers in my belly";
        var stepText = "I have Yellow cucumbers in my belly";
        var expectedParam = (SampleColorEnum.Yellow, typeof(SampleColorEnum));
        var methodName = nameof(SampleBindings.StepDefWithEnumParam);

        var sampleBindings = await PerformStepExecution(methodName, expression, stepText);

        sampleBindings.ExecutedParams.Should().Contain(expectedParam);
    }
}
