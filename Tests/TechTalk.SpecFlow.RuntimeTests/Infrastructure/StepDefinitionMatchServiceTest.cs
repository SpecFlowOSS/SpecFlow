using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using FluentAssertions;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    [TestFixture]
    public class StepDefinitionMatchServiceTest
    {
        private Mock<IBindingRegistry> bindingRegistryMock;
        private Mock<IStepArgumentTypeConverter> stepArgumentTypeConverterMock;
        private readonly CultureInfo bindingCulture = new CultureInfo("en-US");
        private List<IStepDefinitionBinding> whenStepDefinitions;
            
        [SetUp]
        public void Setup()
        {
            whenStepDefinitions = new List<IStepDefinitionBinding>();
            bindingRegistryMock = new Mock<IBindingRegistry>();
            bindingRegistryMock.Setup(r => r.GetConsideredStepDefinitions(StepDefinitionType.When, It.IsAny<string>()))
                .Returns(whenStepDefinitions);

            stepArgumentTypeConverterMock = new Mock<IStepArgumentTypeConverter>();
        }

        private StepDefinitionMatchService CreateSUT()
        {
            return new StepDefinitionMatchService(bindingRegistryMock.Object, stepArgumentTypeConverterMock.Object);
        }

        private static BindingMethod CreateBindingMethod(string name = "dummy")
        {
            return new BindingMethod(new BindingType("dummy", "dummy", "dummy"), name, new IBindingParameter[0], null);
        }

        private static BindingMethod CreateBindingMethodWithStrignParam(string name = "dummy")
        {
            return new BindingMethod(new BindingType("dummy", "dummy", "dummy"), name, new IBindingParameter[] { new BindingParameter(new RuntimeBindingType(typeof(string)), "param1") }, null);
        }

        private StepInstance CreateSimpleWhen(string text = "I do something")
        {
            var result = new StepInstance(StepDefinitionType.When, StepDefinitionKeyword.When, "When ", text, null, null, 
                new StepContext("MyFeature", "MyScenario", new string[0], new CultureInfo("en-US")));
            return result;
        }

        [Test]
        public void Should_GetBestMatch_succeed_when_proper_match()
        {
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, ".*", CreateBindingMethod(), null));

            var sut = CreateSUT();

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            var result = sut.GetBestMatch(CreateSimpleWhen(), bindingCulture, out ambiguityReason, out candidatingMatches);

            result.Success.Should().BeTrue();
        }

        [Test]
        public void Should_GetBestMatch_succeed_when_proper_match_and_non_matching_scopes()
        {
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, ".*", CreateBindingMethod("m1"), null));
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, ".*", CreateBindingMethod("m2"), new BindingScope("non-matching-tag", null, null)));

            var sut = CreateSUT();

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            var result = sut.GetBestMatch(CreateSimpleWhen(), bindingCulture, out ambiguityReason, out candidatingMatches);

            result.Success.Should().BeTrue();
        }

        [Test]
        public void Should_GetBestMatch_succeed_when_proper_match_with_parameters()
        {
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, "(.*)", CreateBindingMethodWithStrignParam(), null));

            var sut = CreateSUT();

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            var result = sut.GetBestMatch(CreateSimpleWhen(), bindingCulture, out ambiguityReason, out candidatingMatches);

            result.Success.Should().BeTrue();
        }

        [Test]
        public void Should_GetBestMatch_fail_when_scope_errors_with_single_match()
        {
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, ".*", CreateBindingMethod(), new BindingScope("non-matching-tag", null, null)));

            var sut = CreateSUT();

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            var result = sut.GetBestMatch(CreateSimpleWhen(), bindingCulture, out ambiguityReason, out candidatingMatches);

            result.Success.Should().BeFalse();
            ambiguityReason.Should().Be(StepDefinitionAmbiguityReason.AmbiguousScopes);
        }

        [Test]
        public void Should_GetBestMatch_fail_when_scope_errors_with_multiple_matches()
        {
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, ".*", CreateBindingMethod("dummy1"), new BindingScope("non-matching-tag", null, null)));
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, ".*", CreateBindingMethod("dummy2"), new BindingScope("other-non-matching-tag", null, null)));

            var sut = CreateSUT();

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            var result = sut.GetBestMatch(CreateSimpleWhen(), bindingCulture, out ambiguityReason, out candidatingMatches);

            result.Success.Should().BeFalse();
            ambiguityReason.Should().Be(StepDefinitionAmbiguityReason.AmbiguousScopes);
        }

        [Test] // in case of single parameter error, we pretend success - the error will be displayed runtime
        public void Should_GetBestMatch_succeed_when_paramter_errors_with_single_match()
        {
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, "(.*)", CreateBindingMethod(), null));

            var sut = CreateSUT();

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            var result = sut.GetBestMatch(CreateSimpleWhen(), bindingCulture, out ambiguityReason, out candidatingMatches);

            result.Success.Should().BeTrue(); 
        }

        [Test]
        public void Should_GetBestMatch_fail_when_parameter_errors_with_multiple_matches()
        {
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, "(.*)", CreateBindingMethod("dummy1"), null));
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, "(.*)", CreateBindingMethod("dummy2"), null));

            var sut = CreateSUT();

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            var result = sut.GetBestMatch(CreateSimpleWhen(), bindingCulture, out ambiguityReason, out candidatingMatches);

            result.Success.Should().BeFalse();
            ambiguityReason.Should().Be(StepDefinitionAmbiguityReason.ParameterErrors);
        }

        [Test]
        public void Should_GetBestMatch_fail_when_multiple_matches()
        {
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, ".*", CreateBindingMethod("dummy1"), null));
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, ".*", CreateBindingMethod("dummy2"), null));

            var sut = CreateSUT();

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            var result = sut.GetBestMatch(CreateSimpleWhen(), bindingCulture, out ambiguityReason, out candidatingMatches);

            result.Success.Should().BeFalse();
            ambiguityReason.Should().Be(StepDefinitionAmbiguityReason.AmbiguousSteps);
        }

        [Test]
        public void Should_GetBestMatch_succeed_when_multiple_matches_are_on_the_same_method()
        {
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, ".*", CreateBindingMethod("dummy"), null));
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, ".*", CreateBindingMethod("dummy"), null));

            var sut = CreateSUT();

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            var result = sut.GetBestMatch(CreateSimpleWhen(), bindingCulture, out ambiguityReason, out candidatingMatches);

            result.Success.Should().BeTrue();
        }

        [Test]
        public void Should_GetBestMatch_succeed_when_no_matching_step_definitions()
        {
            whenStepDefinitions.Add(new StepDefinitionBinding(StepDefinitionType.When, "non-maching-regex", CreateBindingMethod(), null));

            var sut = CreateSUT();

            StepDefinitionAmbiguityReason ambiguityReason;
            List<BindingMatch> candidatingMatches;
            var result = sut.GetBestMatch(CreateSimpleWhen(), bindingCulture, out ambiguityReason, out candidatingMatches);

            result.Success.Should().BeFalse();
            ambiguityReason.Should().Be(StepDefinitionAmbiguityReason.None);
        }
    }
}
