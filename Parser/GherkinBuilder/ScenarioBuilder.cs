using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class ScenarioBuilder : IScenarioBuilder
    {
        private readonly string name;
        private readonly FilePosition position;
        private readonly Tags tags;
        private readonly IList<StepBuilder> steps = new List<StepBuilder>();

        public ScenarioBuilder(string name, Tags tags, FilePosition position)
        {
            this.name = name;
            this.position = position;
            this.tags = tags;
        }

        public void ProcessStep(StepBuilder step)
        {
            steps.Add(step);
        }

        public Scenario GetResult()
        {
            return new Scenario(name, tags, new ScenarioSteps(steps.Select(s => s.GetResult()).ToArray())) { FilePosition = position };
        }
    }
}