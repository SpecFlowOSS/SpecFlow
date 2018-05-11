using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SemanticValidators
{
    internal class DuplicateScenariosValidator : ISemanticValidator
    {
        public List<SemanticParserException> Validate(SpecFlowFeature feature)
        {
            var errors = new List<SemanticParserException>();
            var duplicatedScenarios = feature.ScenarioDefinitions.GroupBy(sd => sd.Name, sd => sd).Where(g => g.Count() > 1).ToArray();
            errors.AddRange(
                duplicatedScenarios.Select(g =>
                    new SemanticParserException(
                        string.Format("Feature file already contains a scenario with name '{0}'", g.Key),
                        g.ElementAt(1).Location)));

            return errors;
        }
    }
}