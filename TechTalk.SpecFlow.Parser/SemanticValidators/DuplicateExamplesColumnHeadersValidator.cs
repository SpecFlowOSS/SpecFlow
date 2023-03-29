using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SemanticValidators
{
    internal class DuplicateExamplesColumnHeadersValidator : ISemanticValidator
    {
        public List<SemanticParserException> Validate(SpecFlowFeature feature)
        {
            var errors = new List<SemanticParserException>();
            foreach (var scenarioDefinition in feature.ScenarioDefinitions)
            {
                var scenarioOutline = scenarioDefinition as ScenarioOutline;
                if (scenarioOutline != null)
                {
                    foreach (var example in scenarioOutline.Examples)
                    {
                        if (example.TableHeader != null)
                        {
                            var duplicateExamples = example.TableHeader.Cells
                                                           .GroupBy(g => g.Value)
                                                           .Where(g => g.Count() > 1);

                            foreach (var duplicateExample in duplicateExamples)
                            {
                                var message = $"Scenario Outline '{scenarioOutline.Name}' already contains an example column with header '{duplicateExample.Key}'";
                                errors.Add(new SemanticParserException(message, scenarioOutline.Location));
                            }
                        }
                    }
                }
            }

            return errors;
        } 
    }
}
