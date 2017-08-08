using System;
using System.Collections.Generic;
using System.Globalization;
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
    [TestFixture]
    public class StepDefinitionRegexCalculatorTests
    {
        private SpecFlowConfiguration specFlowConfiguration;

        [SetUp]
        public void Setup()
        {
            specFlowConfiguration = ConfigurationLoader.GetDefault();
        }

        private StepDefinitionRegexCalculator CreateSut()
        {
            return new StepDefinitionRegexCalculator(specFlowConfiguration);
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
            match.Success.Should().BeTrue("the calculated regex ({0}) should match text <{1}>", regex.ToString().Replace("{", "<").Replace("}", ">"), text);
            return match;
        }

        private void AssertParamMatch(Match match, string paramMatch, int paramIndex = 0)
        {
            match.Groups.Count.Should().BeGreaterThan(paramIndex, $"there should be a parameter #{paramIndex} in the regex match");
            match.Groups[paramIndex + 1].Value.Should().Be(paramMatch, $"parameter #{paramIndex} should be <{paramMatch}>");
        }

        [TestCase("When_I_do_something")]
        [TestCase("WhenIDoSomething")]
        [TestCase("When_I_doSomething")] //mixed
        public void RecognizeSimpleText(string methodName)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When, 
                CreateBindingMethod(methodName));

            AssertMatches(result, "I do something");
        }

        [Test, Combinatorial]
        public void RecognizeParametrizedText_ParamInMiddle(
            [Values("When_that_WHO_does_something", "WhenThatWHODoesSomething", "WhenThat_WHO_DoesSomething")] string methodName,
            [Values("Joe", "'Joe'", "\"Joe\"")] string paramInStepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When, 
                CreateBindingMethod(methodName, "who"));

            var match = AssertMatches(result, $"that {paramInStepText} does something");
            AssertParamMatch(match, "Joe");
        }

        [Test, Combinatorial]
        public void RecognizeParametrizedText_ParamInFront(
            [Values("When_WHO_does_something", "WhenWHODoesSomething", "When_WHO_DoesSomething")] string methodName,
            [Values("Joe", "'Joe'", "\"Joe\"")] string paramInStepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When, 
                CreateBindingMethod(methodName, "who"));

            var match = AssertMatches(result, $"{paramInStepText} does something");
            AssertParamMatch(match, "Joe");
        }

        [Test, Combinatorial]
        public void RecognizeParametrizedText_ParamAtTheEnd(
            [Values("Given_user_WHO", "GivenUserWHO", "GivenUser_WHO")] string methodName,
            [Values("Joe", "'Joe'", "\"Joe\"")] string paramInStepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.Given, 
                CreateBindingMethod(methodName, "who"));

            var match = AssertMatches(result, $"user {paramInStepText}");
            AssertParamMatch(match, "Joe");
        }

        [Test, Combinatorial]
        public void RecognizeParametrizedText_UnderscoreInParamName(
            [Values("When_that_W_H_O_does_something", "WhenThatW_H_ODoesSomething", "WhenThat_W_H_O_DoesSomething")] string methodName,
            [Values("Joe", "'Joe'", "\"Joe\"")] string paramInStepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName, "w_h_o"));

            var match = AssertMatches(result, $"that {paramInStepText} does something");
            AssertParamMatch(match, "Joe");
        }

        [Test, Combinatorial]
        public void RecognizeParametrizedText_NumberParams(
            [Values("When_using_VALUE_as_parameter", "WhenUsingVALUEAsParameter", "WhenUsing_VALUE_AsParameter")] string methodName,
            [Values("1", "123", "-123", "12.3", "£123")] string paramText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName, "value"));

            var match = AssertMatches(result, $"using {paramText} as parameter");
            AssertParamMatch(match, paramText);
        }

        [Test]
        public void SupportsExtraArguments()
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod("When_WHO_does_something", "who", "table"));

            AssertMatches(result, "Joe does something");
        }

        [TestCase("that:Joe,does;something.?!")]
        [TestCase("that : Joe , does ; something .?! ")]
        [TestCase("that -Joe - does -something-")]
        [TestCase("that' Joe does \"something\"")]
        public void SupportsPunctuation(string stepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod("When_that_WHO_does_something", "who"));

            var match = AssertMatches(result, stepText);
            AssertParamMatch(match, "Joe");
        }

        [TestCase("!that does not work")]
        [TestCase(" that does not work")]
        public void DoesNotSupportWhitespaceOrPunctuationInFrontOfStepText(string stepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod("When_that_does_not_work", "who"));

            var match = result.Match(stepText);
            match.Success.Should().BeFalse();
        }

        [TestCase("this doesn't work", "this_doesnt_work", false)]
        [TestCase("this doesn't work", "this_doesn_t_work", true)]
        [TestCase("this does not work", "this_does_not_work", true)]
        public void DoesNotSupportApostrophedShortenings(string stepText, string methodName, bool shouldMatch)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName));

            var match = result.Match(stepText);
            match.Success.Should().Be(shouldMatch);
        }

        [TestCase("When_WHO_does_WHAT_with")]
        [TestCase("WhenWHODoesWHATWith")]
        [TestCase("When_WHO_Does_WHAT_With")]
        [TestCase("When_P0_does_P1_with")]
        public void SupportsMultipleParameters(string methodName)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName, "who", "what"));

            var match = AssertMatches(result, "Joe does something with");
            AssertParamMatch(match, "Joe", 0);
            AssertParamMatch(match, "something", 1);
        }

        [TestCase("(.*) does something with")]
        public void SupportsRegexMethodNames(string methodName)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName, "who"));

            result.ToString().Should().Be("^" + methodName + "$");

            var match = AssertMatches(result, "Joe does something with");
            AssertParamMatch(match, "Joe");
        }

        [TestCase("I_do_something")]
        [TestCase("IDoSomething")]
        public void KeywordCanBeAvoided(string methodName)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName));

            AssertMatches(result, "I do something");
        }

        [TestCase("Angenommen_ich_Knopf_drücke")]
        [TestCase("Gegeben_sei_ich_Knopf_drücke")]
        [TestCase("Given_ich_Knopf_drücke")]
        public void LocalizedKeywordCanBeUsedIfFeatureLanguageIsConfigured(string methodName)
        {
            specFlowConfiguration.FeatureLanguage = new CultureInfo("de-AT");
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.Given,
                CreateBindingMethod(methodName));

            AssertMatches(result, "ich Knopf drücke");
        }

        [TestCase("Допустим_я_не_авторизован")]
        [TestCase("я_не_авторизован")]
        [TestCase("Given_я_не_авторизован")]
        public void Issue715(string methodName)
        {
            specFlowConfiguration.FeatureLanguage = new CultureInfo("ru");
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.Given,
                CreateBindingMethod(methodName));

            AssertMatches(result, "я не авторизован");
        }
    }
}
