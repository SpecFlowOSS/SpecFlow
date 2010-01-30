using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class ScenarioOutlineBuilder : IScenarioBuilder, IExampleProcessor
    {
        private readonly string name;
        private readonly Tags tags;
        private readonly FilePosition position;
        private readonly IList<StepBuilder> steps = new List<StepBuilder>();
        private readonly IList<ExampleBuilder> examples = new List<ExampleBuilder>();

        public ScenarioOutlineBuilder(string name, Tags tags, FilePosition position)
        {
            this.name = name;
            this.tags = tags;
            this.position = position;
        }

        public void ProcessStep(StepBuilder step)
        {
            steps.Add(step);
        }

        public Scenario GetResult()
        {
            return new ScenarioOutline(
                new Text(name),
                tags,
                new ScenarioSteps(steps.Select(step => step.GetResult()).ToArray()),
                new Examples(examples.Select(example => example.GetResult()).ToArray())) { FilePosition = position };
        }

        public void ProcessExample(ExampleBuilder examplebuilder)
        {
            examples.Add(examplebuilder);
        }
    }
}