using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings
{
    [TestFixture]
    public class BindingRegistryTests
    {
        [Test]
        public void GetStepDefinitions_should_return_all_step_definitions()
        {
            var sut = new BindingRegistry();

            var stepDefinitionBinding1 = new StepDefinitionBinding(StepDefinitionType.Given, @"foo.*", new Mock<IBindingMethod>().Object, null);
            var stepDefinitionBinding2 = new StepDefinitionBinding(StepDefinitionType.When, @"bar.*", new Mock<IBindingMethod>().Object, null);
            sut.RegisterStepDefinitionBinding(stepDefinitionBinding1);
            sut.RegisterStepDefinitionBinding(stepDefinitionBinding2);

            var result = sut.GetStepDefinitions();

            result.Should().BeEquivalentTo(stepDefinitionBinding1, stepDefinitionBinding2);
        }

        [Test]
        public void GetHooks_should_return_all_hooks()
        {
            var sut = new BindingRegistry();

            var hook1 = new HookBinding(new Mock<IBindingMethod>().Object, HookType.BeforeScenario, null, 1);
            var hook2 = new HookBinding(new Mock<IBindingMethod>().Object, HookType.AfterFeature, null, 2);
            sut.RegisterHookBinding(hook1);
            sut.RegisterHookBinding(hook2);

            var result = sut.GetHooks();

            result.Should().BeEquivalentTo(hook1, hook2);
        }
    }
}
