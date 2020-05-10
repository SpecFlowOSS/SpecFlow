using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;
using TechTalk.SpecFlow.BindingSkeletons;
using FluentAssertions;

namespace TechTalk.SpecFlow.RuntimeTests.BindingSkeletons
{

    public class StepTextAnalyzerTests
    {
        private readonly CultureInfo bindingCulture = new CultureInfo("en-US", false);

        [Fact]
        public void Should_not_change_simple_step()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I do something", bindingCulture);
            result.Parameters.Should().BeEmpty();
            result.TextParts.Count.Should().Be(1);
            result.TextParts[0].Should().Be("I do something");
        }

        [Fact]
        public void Should_recognize_quoted_strings()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I \"did\" something", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.Parameters[0].Name.Should().Be("did");
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I \"");
            result.TextParts[1].Should().Be("\" something");
        }

        [Fact]
        public void Should_recognize_apostrophed_strings()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I 'did' something", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.Parameters[0].Name.Should().Be("did");
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I '");
            result.TextParts[1].Should().Be("' something");
        }

        [Fact]
        public void Should_recognize_angle_bracket_strings()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I <did> something", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.Parameters[0].Name.Should().Be("did");
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I ");
            result.TextParts[1].Should().Be(" something");
        }

        [Fact]
        public void Should_handle_quote_overlaps()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I 'do \" something' really \" strange", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.Parameters[0].Name.Should().Be("p0");
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I '");
            result.TextParts[1].Should().Be("' really \" strange");
        }

        [Fact]
        public void Should_handle_overlaps_with_numbers()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I 'do 42 something' foo", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I '");
            result.TextParts[1].Should().Be("' foo");
        }

        [Fact]
        public void Should_recognize_integers()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I have 42 bars", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.Parameters[0].Name.Should().Be("p0");
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I have ");
            result.TextParts[1].Should().Be(" bars");
            result.Parameters[0].Type.Should().Be("Int32");
        }

        [Theory]
        [InlineData("2030-12-23", "en-US")]
        [InlineData("2030/12/23", "en-US")]
        [InlineData("23-12-2030", "nl-NL")]
        [InlineData("23.12.2030", "nl-BE")]
        public void Should_recognize_dates(string dateString, string cultureCode)
        {
            var sut = new StepTextAnalyzer();

            var culture = CultureInfo.GetCultureInfo(cultureCode);

            var result = sut.Analyze("Zombie apocalypse is expected at " + dateString, culture);
            result.Parameters.Count.Should().Be(1);
            result.Parameters[0].Name.Should().Be("p0");
            result.Parameters[0].Type.Should().Be("DateTime");
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("Zombie apocalypse is expected at ");
            result.TextParts[1].Should().Be("");
        }

        [Fact]
        public void Should_recognize_decimals()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I have 4.2 bars", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.Parameters[0].Name.Should().Be("p0");
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I have ");
            result.TextParts[1].Should().Be(" bars");
            result.Parameters[0].Type.Should().Be("Decimal");
        }

        [Fact]
        public void Should_recognize_quoted_strings_with_multiple_parameters()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I \"did\" something with \"multiple\" parameters", bindingCulture);
            result.Parameters.Count.Should().Be(2);
            result.Parameters[0].Name.Should().Be("did");
            result.Parameters[1].Name.Should().Be("multiple");
            result.TextParts.Count.Should().Be(3);
            result.TextParts[0].Should().Be("I \"");
            result.TextParts[1].Should().Be("\" something with \"");
            result.TextParts[2].Should().Be("\" parameters");
        }

        [Fact]
        public void Should_not_use_smart_parameter_names_when_they_contain_spaces()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I \"do spaced parameter\" something", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.Parameters[0].Name.Should().Be("p0");
        }

        [Fact]
        public void Should_not_use_smart_parameter_names_when_they_contain_non_alphabet_characters()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I \"do?\" something", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.Parameters[0].Name.Should().Be("p0");
        }

        [Fact]
        public void Should_not_use_smart_parameter_names_when_they_contain_numeric_characters()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I \"do1\" something", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.Parameters[0].Name.Should().Be("p0");
        }

        [Fact]
        public void Should_not_use_same_parameter_names_when_they_appear_multiple_times()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I \"did\" something and \"did\" something else", bindingCulture);
            result.Parameters.Count.Should().Be(2);
            result.Parameters[0].Name.Should().Be("did");
            result.Parameters[1].Name.Should().Be("did1");
        }

        [Fact]
        public void Should_use_the_correct_param_index_when_they_appear_multiple_times()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I \"did\" something \"and\" then \"did\" something else", bindingCulture);
            result.Parameters.Count.Should().Be(3);
            result.Parameters[0].Name.Should().Be("did");
            result.Parameters[1].Name.Should().Be("and");
            result.Parameters[2].Name.Should().Be("did2");
        }

        [Fact]
        public void Should_correctly_case_parameter_names()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I \"Did\" something with \"MUlTiPLe\" parameters", bindingCulture);
            result.Parameters.Count.Should().Be(2);
            result.Parameters[0].Name.Should().Be("did");
            result.Parameters[1].Name.Should().Be("mUlTiPLe");
            result.TextParts.Count.Should().Be(3);
            result.TextParts[0].Should().Be("I \"");
            result.TextParts[1].Should().Be("\" something with \"");
            result.TextParts[2].Should().Be("\" parameters");
        }

        [Fact]
        public void Should_support_accented_characters()
        {
            var sut = new StepTextAnalyzer();

            var result = sut.Analyze("I \"dö\" something ", bindingCulture);
            result.Parameters.Count.Should().Be(1);
            result.Parameters[0].Name.Should().Be("dö");
            result.TextParts.Count.Should().Be(2);
            result.TextParts[0].Should().Be("I \"");
            result.TextParts[1].Should().Be("\" something ");
        }
    }
}
