using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings
{
    
    public class StepDefinitionRegexCalculatorTests
    {
        private SpecFlowConfiguration specFlowConfiguration;

        public StepDefinitionRegexCalculatorTests()
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
                new BindingType("SomeAssembly", "SomeSteps", "SomeSteps"),
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

        [Theory]
        [InlineData("When_I_do_something")]
        [InlineData("WhenIDoSomething")]
        [InlineData("When_I_doSomething")] //mixed
        public void RecognizeSimpleText(string methodName)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When, 
                CreateBindingMethod(methodName));

            AssertMatches(result, "I do something");
        }

        [Theory]
        [InlineData("When_that_WHO_does_something", "Joe")]
        [InlineData("When_that_WHO_does_something", "'Joe'")]
        [InlineData("When_that_WHO_does_something", "\"Joe\"")]
        [InlineData("WhenThatWHODoesSomething", "Joe")]
        [InlineData("WhenThatWHODoesSomething", "'Joe'")]
        [InlineData("WhenThatWHODoesSomething", "\"Joe\"")]
        [InlineData("WhenThat_WHO_DoesSomething", "Joe")]
        [InlineData("WhenThat_WHO_DoesSomething", "'Joe'")]
        [InlineData("WhenThat_WHO_DoesSomething", "\"Joe\"")]
        public void RecognizeParametrizedText_ParamInMiddle(string methodName, string paramInStepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When, 
                CreateBindingMethod(methodName, "who"));

            var match = AssertMatches(result, $"that {paramInStepText} does something");
            AssertParamMatch(match, "Joe");
        }

        [Theory]
        [InlineData("When_WHO_does_something", "Joe")]
        [InlineData("When_WHO_does_something", "'Joe'")]
        [InlineData("When_WHO_does_something", "\"Joe\"")]
        [InlineData("WhenWHODoesSomething", "Joe")]
        [InlineData("WhenWHODoesSomething", "'Joe'")]
        [InlineData("WhenWHODoesSomething", "\"Joe\"")]
        [InlineData("When_WHO_DoesSomething", "Joe")]
        [InlineData("When_WHO_DoesSomething", "'Joe'")]
        [InlineData("When_WHO_DoesSomething", "\"Joe\"")]
        public void RecognizeParametrizedText_ParamInFront(string methodName,string paramInStepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When, 
                CreateBindingMethod(methodName, "who"));

            var match = AssertMatches(result, $"{paramInStepText} does something");
            AssertParamMatch(match, "Joe");
        }

        [Theory]
        [InlineData("Given_user_WHO", "Joe")]
        [InlineData("Given_user_WHO", "'Joe'")]
        [InlineData("Given_user_WHO", "\"Joe\"")]
        [InlineData("GivenUserWHO", "Joe")]
        [InlineData("GivenUserWHO", "'Joe'")]
        [InlineData("GivenUserWHO", "\"Joe\"")]
        [InlineData("GivenUser_WHO", "Joe")]
        [InlineData("GivenUser_WHO", "'Joe'")]
        [InlineData("GivenUser_WHO", "\"Joe\"")]
        public void RecognizeParametrizedText_ParamAtTheEnd(string methodName, string paramInStepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.Given, 
                CreateBindingMethod(methodName, "who"));

            var match = AssertMatches(result, $"user {paramInStepText}");
            AssertParamMatch(match, "Joe");
        }

        [Theory]
        [InlineData("When_that_W_H_O_does_something", "Joe")]
        [InlineData("When_that_W_H_O_does_something", "'Joe'")]
        [InlineData("When_that_W_H_O_does_something", "\"Joe\"")]
        [InlineData("WhenThatW_H_ODoesSomething", "Joe")]
        [InlineData("WhenThatW_H_ODoesSomething", "'Joe'")]
        [InlineData("WhenThatW_H_ODoesSomething", "\"Joe\"")]
        [InlineData("WhenThat_W_H_O_DoesSomething", "Joe")]
        [InlineData("WhenThat_W_H_O_DoesSomething", "'Joe'")]
        [InlineData("WhenThat_W_H_O_DoesSomething", "\"Joe\"")]
        public void RecognizeParametrizedText_UnderscoreInParamName(string methodName, string paramInStepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName, "w_h_o"));

            var match = AssertMatches(result, $"that {paramInStepText} does something");
            AssertParamMatch(match, "Joe");
        }

        [Theory]
        [InlineData("When_using_VALUE_as_parameter", "1")]
        [InlineData("When_using_VALUE_as_parameter", "123")]
        [InlineData("When_using_VALUE_as_parameter", "-123")]
        [InlineData("When_using_VALUE_as_parameter", "12.3")]
        [InlineData("When_using_VALUE_as_parameter", "£123")]
        [InlineData("WhenUsingVALUEAsParameter", "1")]
        [InlineData("WhenUsingVALUEAsParameter", "123")]
        [InlineData("WhenUsingVALUEAsParameter", "-123")]
        [InlineData("WhenUsingVALUEAsParameter", "12.3")]
        [InlineData("WhenUsingVALUEAsParameter", "£123")]
        [InlineData("WhenUsing_VALUE_AsParameter", "1")]
        [InlineData("WhenUsing_VALUE_AsParameter", "123")]
        [InlineData("WhenUsing_VALUE_AsParameter", "-123")]
        [InlineData("WhenUsing_VALUE_AsParameter", "12.3")]
        [InlineData("WhenUsing_VALUE_AsParameter", "£123")]
        public void RecognizeParametrizedText_NumberParams(string methodName, string paramText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName, "value"));

            var match = AssertMatches(result, $"using {paramText} as parameter");
            AssertParamMatch(match, paramText);
        }

        [Fact]
        public void SupportsExtraArguments()
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod("When_WHO_does_something", "who", "table"));

            AssertMatches(result, "Joe does something");
        }
        
        [Theory]
        [InlineData("that:Joe,does;something.?!")]
        [InlineData("that : Joe , does ; something .?! ")]
        [InlineData("that -Joe - does -something-")]
        [InlineData("that' Joe does \"something\"")]
        public void SupportsPunctuation(string stepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod("When_that_WHO_does_something", "who"));

            var match = AssertMatches(result, stepText);
            AssertParamMatch(match, "Joe");
        }

        [Theory]
        [InlineData("!that does not work")]
        [InlineData(" that does not work")]
        public void DoesNotSupportWhitespaceOrPunctuationInFrontOfStepText(string stepText)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod("When_that_does_not_work", "who"));

            var match = result.Match(stepText);
            match.Success.Should().BeFalse();
        }

        [Theory]
        [InlineData("this doesn't work", "this_doesnt_work", false)]
        [InlineData("this doesn't work", "this_doesn_t_work", true)]
        [InlineData("this does not work", "this_does_not_work", true)]
        public void DoesNotSupportApostrophedShortenings(string stepText, string methodName, bool shouldMatch)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName));

            var match = result.Match(stepText);
            match.Success.Should().Be(shouldMatch);
        }

        [Theory]
        [InlineData("When_WHO_does_WHAT_with")]
        [InlineData("WhenWHODoesWHATWith")]
        [InlineData("When_WHO_Does_WHAT_With")]
        [InlineData("When_P0_does_P1_with")]
        public void SupportsMultipleParameters(string methodName)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName, "who", "what"));

            var match = AssertMatches(result, "Joe does something with");
            AssertParamMatch(match, "Joe", 0);
            AssertParamMatch(match, "something", 1);
        }

        [Theory]
        [InlineData("(.*) does something with")]
        public void SupportsRegexMethodNames(string methodName)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName, "who"));

            result.ToString().Should().Be("^" + methodName + "$");

            var match = AssertMatches(result, "Joe does something with");
            AssertParamMatch(match, "Joe");
        }

        [Theory]
        [InlineData("I_do_something")]
        [InlineData("IDoSomething")]
        public void KeywordCanBeAvoided(string methodName)
        {
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.When,
                CreateBindingMethod(methodName));

            AssertMatches(result, "I do something");
        }

        [Theory]
        [InlineData("Angenommen_ich_Knopf_drücke")]
        [InlineData("Gegeben_sei_ich_Knopf_drücke")]
        [InlineData("Given_ich_Knopf_drücke")]
        public void LocalizedKeywordCanBeUsedIfFeatureLanguageIsConfigured(string methodName)
        {
            specFlowConfiguration.FeatureLanguage = new CultureInfo("de-AT", false);
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.Given,
                CreateBindingMethod(methodName));

            AssertMatches(result, "ich Knopf drücke");
        }

        [Theory]
        [InlineData("Допустим_я_не_авторизован")]
        [InlineData("я_не_авторизован")]
        [InlineData("Given_я_не_авторизован")]
        public void Issue715(string methodName)
        {
            specFlowConfiguration.FeatureLanguage = new CultureInfo("ru", false);
            var sut = CreateSut();

            var result = CallCalculateRegexFromMethodAndAssertRegex(sut, StepDefinitionType.Given,
                CreateBindingMethod(methodName));

            AssertMatches(result, "я не авторизован");
        }
    }
}
