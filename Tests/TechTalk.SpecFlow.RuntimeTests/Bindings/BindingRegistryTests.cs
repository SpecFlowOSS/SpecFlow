using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Xunit;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings
{
    
    public class BindingRegistryTests
    {
        [Fact]
        public void GetStepDefinitions_should_return_all_step_definitions()
        {
            var sut = new BindingRegistry();

            var stepDefinitionBinding1 = StepDefinitionHelper.CreateRegex(StepDefinitionType.Given, @"foo.*");
            var stepDefinitionBinding2 = StepDefinitionHelper.CreateRegex(StepDefinitionType.When, @"bar.*");
            sut.RegisterStepDefinitionBinding(stepDefinitionBinding1);
            sut.RegisterStepDefinitionBinding(stepDefinitionBinding2);

            var result = sut.GetStepDefinitions();

            result.Should().BeEquivalentTo(new List<IStepDefinitionBinding> { stepDefinitionBinding1, stepDefinitionBinding2 });
        }

        [Fact]
        public void GetHooks_should_return_all_hooks()
        {
            var sut = new BindingRegistry();

            var hook1 = new HookBinding(new Mock<IBindingMethod>().Object, HookType.BeforeScenario, null, 1);
            var hook2 = new HookBinding(new Mock<IBindingMethod>().Object, HookType.AfterFeature, null, 2);
            sut.RegisterHookBinding(hook1);
            sut.RegisterHookBinding(hook2);

            var result = sut.GetHooks();

            result.Should().BeEquivalentTo(new List<HookBinding> { hook1, hook2 });
        }
    }
}
