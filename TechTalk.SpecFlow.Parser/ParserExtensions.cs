using System;
using System.Collections.Generic;
using System.Linq;
using Gherkin;
using Gherkin.Ast;

namespace TechTalk.SpecFlow.Parser
{
    public static class ParserExtensions
    {
        public static ParserException[] GetParserExceptions(this ParserException parserException)
        {
            var composite = parserException as CompositeParserException;
            if (composite != null)
                return composite.Errors.ToArray();

            return new[] { parserException };
        }

        public static ScenarioBlock? ToScenarioBlock(this StepKeyword stepKeyword)
        {
            switch (stepKeyword)
            {
                case StepKeyword.Given:
                    return ScenarioBlock.Given;
                case StepKeyword.When:
                    return ScenarioBlock.When;
                case StepKeyword.Then:
                    return ScenarioBlock.Then;
            }
            return null;
        }

        public static string GetNameWithoutAt(this Tag tag)
        {
            return tag.Name.TrimStart('@');
        }

        public static bool HasTags(this IHasTags hasTags)
        {
            return hasTags.Tags.Any();
        }

        public static bool HasTags(this StepsContainer scenarioDefinition)
        {
            var hasTags = scenarioDefinition as IHasTags;
            if (hasTags == null)
                return false;

            return hasTags.HasTags();
        }

        public static IEnumerable<Tag> GetTags(this StepsContainer scenarioDefinition)
        {
            if (!scenarioDefinition.HasTags())
            {
                return new List<Tag>();
            }

            return ((IHasTags)scenarioDefinition).Tags;
        }     
    }

    public static class SpecFlowLocation
    {
        public static Location Empty { get; } = new Location();
    }
}
