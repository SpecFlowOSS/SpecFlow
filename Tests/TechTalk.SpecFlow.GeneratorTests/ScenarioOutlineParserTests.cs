using FluentAssertions;
using Gherkin;
using Gherkin.Ast;
using Xunit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public class ScenarioOutlineParserTests
    {
        [Fact]
        public void Parser_throws_meaningful_exception_when_Examples_are_missing_in_Scenario_Outline()
        {
            var feature = @"Feature: Missing
                            Scenario Outline: No Examples";

            var parser = new SpecFlowGherkinParser(CultureInfo.GetCultureInfo("en"));

            Action act = () => parser.Parse(new StringReader(feature), null);

            act.Should().Throw<SemanticParserException>().WithMessage("(2:29): Scenario Outline 'No Examples' has no examples defined")
                .And.Location.Line.Should().Be(2);
        }

        [Fact]
        public void Parser_throws_meaningful_exception_when_Examples_are_empty_in_Scenario_Outline()
        {
            var feature = @"Feature: Missing
                            Scenario Outline: No Examples
                            Given something
                            
                            Examples:";

            var parser = new SpecFlowGherkinParser(CultureInfo.GetCultureInfo("en"));

            Action act = () => parser.Parse(new StringReader(feature), null);

            act.Should().Throw<SemanticParserException>().WithMessage("(2:29): Scenario Outline 'No Examples' has no examples defined")
                .And.Location.Line.Should().Be(2);
        }

        [Fact]
        public void Parser_throws_meaningful_exception_when_Examples_have_header_but_are_empty_in_Scenario_Outline()
        {
            var feature = @"Feature: Missing
                            Scenario Outline: No Examples
                            Given something
                            
                            Examples:
                            | Column |
                            ";

            var parser = new SpecFlowGherkinParser(CultureInfo.GetCultureInfo("en"));

            Action act = () => parser.Parse(new StringReader(feature), null);

            act.Should().Throw<SemanticParserException>().WithMessage("(2:29): Scenario Outline 'No Examples' has no examples defined")
                .And.Location.Line.Should().Be(2);
        }

        [Fact]
        public void Parser_throws_meaningful_exception_when_Examples_are_missing_in_multiple_Scenario_Outlines()
        {
            var feature = @"Feature: Missing
                            Scenario Outline: No Examples
                            Scenario Outline: Still no Examples";

            var parser = new SpecFlowGherkinParser(CultureInfo.GetCultureInfo("en"));

            Action act = () => parser.Parse(new StringReader(feature), null);

            var expectedErrors = new List<SemanticParserException> { new SemanticParserException("Scenario Outline 'No Examples' has no examples defined", new Location(2, 29)),
                new SemanticParserException("Scenario Outline 'Still no Examples' has no examples defined", new Location(3, 29))};

            act.Should().Throw<CompositeParserException>().And.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public void Parser_doesnt_throw_exception_when_Examples_are_provided_for_Scenario_Outline()
        {
            var feature = @"Feature: Missing
                    Scenario Outline: No Examples
                    Given I do <thing>
                    Examples:
                    | thing |
                    | test  |";

            var parser = new SpecFlowGherkinParser(CultureInfo.GetCultureInfo("en"));

            Action act = () => parser.Parse(new StringReader(feature), null);

            act.Should().NotThrow();
        }
    }
}
