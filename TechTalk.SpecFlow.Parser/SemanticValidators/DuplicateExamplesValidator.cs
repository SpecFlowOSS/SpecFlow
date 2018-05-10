using Gherkin.Ast;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SemanticValidators
{
    internal class DuplicateExamplesValidator : ISemanticValidator
    {
        public List<SemanticParserException> Validate(SpecFlowFeature feature)
        {
            var errors = new List<SemanticParserException>();
            foreach (var scenarioDefinition in feature.ScenarioDefinitions)
            {
                var scenarioOutline = scenarioDefinition as ScenarioOutline;
                if (scenarioOutline != null)
                {
                    var duplicateExamples = scenarioOutline.Examples
                                                           .Where(e => !String.IsNullOrWhiteSpace(e.Name))
                                                           .Where(e => e.Tags.All(t => t.Name != "ignore"))
                                                           .GroupBy(e => e.Name, e => e).Where(g => g.Count() > 1);

                    foreach (var duplicateExample in duplicateExamples)
                    {
                        var message = string.Format("Scenario Outline '{0}' already contains an example with name '{1}'", scenarioOutline.Name, duplicateExample.Key);
                        var semanticParserException = new SemanticParserException(message, duplicateExample.ElementAt(1).Location);
                        errors.Add(semanticParserException);
                    }
                }
            }

            return errors;
        }
    }
}