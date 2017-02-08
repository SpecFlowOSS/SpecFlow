using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings
{
    [TestFixture, Category("wip_gn")]
    public class StepDefinitionRegexCalculatorTests
    {
        private StepDefinitionRegexCalculator CreateSut()
        {
            return new StepDefinitionRegexCalculator(new RuntimeConfiguration());
        }

        private IBindingMethod CreateBindingMethod(string name, params string[] parameters)
        {
            parameters = parameters ?? new string[0];
            return new BindingMethod(
                new BindingType("SomeSteps", "SomeSteps"), 
                name, 
                parameters.Select(pn => new BindingParameter(new RuntimeBindingType(typeof(string)), pn)), 
                new RuntimeBindingType(typeof(void)));
        }

        private Regex AssertRegex(string regexText)
        {
            regexText.Should().NotBeNullOrWhiteSpace("null, empty or whitespace-only regex is not valid for step definitions");
            return RegexFactory.Create(regexText); // uses the same regex creation as real step definitions
        }

        private Regex CallCalculateRegexFromMethodAndAssertRegex(StepDefinitionRegexCalculator sut, StepDefinitionType stepDefinitionType, IBindingMethod bindingMethod)
        {
            var result = sut.CalculateRegexFromMethod(stepDefinitionType, bindingMethod);
            return AssertRegex(result);
        }

        private Match AssertMatches(Regex regex, string text)
        {
            var match = regex.Match(text);
            match.Success.Should().BeTrue($"the calculated regex ({regex}) should match text <{text}>");
            return match;
        }

        private void AssertParamMatch(Match match, string paramMatch, int paramIndex = 0)
        {
            match.Groups.Count.Should().BeGreaterThan(paramIndex, $"there should be a parameter #{paramIndex} in the regex match");
            match.Groups[paramIndex + 1].Value.Should().Be(paramMatch, $"parameter #{paramIndex} should be <{paramMatch}>");
        }

        [Test]
        public void RecognizeSimpleText_Underscores()
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When, 
                CreateBindingMethod("When_I_do_something"));

            AssertMatches(result, "I do something");
        }

        [Test]
        public void RecognizeParametrizedText_Underscores()
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When, 
                CreateBindingMethod("When_WHO_does_something", "who"));

            var match = AssertMatches(result, "Joe does something");
            AssertParamMatch(match, "Joe");
        }
    }
}
