using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class ScenarioBuilder : IScenarioBuilder
    {
        private readonly string title;
        private readonly FilePosition position;
        private readonly Tags tags;
        private readonly IList<StepBuilder> steps = new List<StepBuilder>();

        public ScenarioBuilder(string name, string description, Tags tags, FilePosition position)
        {
            this.title = TextHelper.GetText(name, description);
            this.position = position;
            this.tags = tags;
        }

        public void ProcessStep(StepBuilder step)
        {
            steps.Add(step);
        }

        public Scenario GetResult()
        {
            return new Scenario(title, tags, new ScenarioSteps(steps.Select(s => s.GetResult()).ToArray())) { FilePosition = position };
        }
    }
}