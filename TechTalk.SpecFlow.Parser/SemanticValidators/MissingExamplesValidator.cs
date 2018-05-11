using Gherkin.Ast;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SemanticValidators
{
    internal class MissingExamplesValidator : ISemanticValidator
    {
        public List<SemanticParserException> Validate(SpecFlowFeature feature)
        {
            var errors = new List<SemanticParserException>();
            foreach (var scenarioDefinition in feature.ScenarioDefinitions)
            {
                var scenarioOutline = scenarioDefinition as ScenarioOutline;
                if (scenarioOutline != null)
                {
                    if (DoesntHavePopulatedExamples(scenarioOutline))
                    {
                        var message = string.Format("Scenario Outline '{0}' has no examples defined", scenarioOutline.Name);
                        var semanticParserException = new SemanticParserException(message, scenarioDefinition.Location);
                        errors.Add(semanticParserException);
                    }
                }
            }

            return errors;
        }

        private static bool DoesntHavePopulatedExamples(ScenarioOutline scenarioOutline)
        {
            return !scenarioOutline.Examples.Any() || scenarioOutline.Examples.Any(x => x.TableBody == null || !x.TableBody.Any());
        }
    }
}