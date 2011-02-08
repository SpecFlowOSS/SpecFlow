using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class ScenarioOutlineBuilder : ScenarioBuilder, IExampleProcessor
    {
        private readonly IList<ExampleBuilder> examples = new List<ExampleBuilder>();

        public ScenarioOutlineBuilder(string keyword, string name, string description, Tags tags, FilePosition position) :
            base(keyword, name, description, tags, position)
        {
        }

        public override Scenario GetResult()
        {
            if (examples.Count == 0)
                throw new GherkinSemanticErrorException(
                    "There are no examples defined for the scenario outline.", position);

            return new ScenarioOutline(
                keyword,
                title,
                description,
                tags,
                new ScenarioSteps(steps.Select(step => step.GetResult()).ToArray()),
                new Examples(examples.Select(example => example.GetResult()).ToArray())) { FilePosition = position };
        }

        public void ProcessExample(ExampleBuilder examplebuilder)
        {
            if (!string.IsNullOrEmpty(examplebuilder.Title) &&
                examples.Any(s => s.Title.Equals(examplebuilder.Title)))
                throw new GherkinSemanticErrorException(
                    string.Format("Scenario outline already contains an example set name '{0}'", examplebuilder.Title),
                    examplebuilder.Position);

            examples.Add(examplebuilder);
        }
    }
}