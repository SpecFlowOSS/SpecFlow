using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class BackgroundBuilder : IStepProcessor
    {
        private readonly string text;
        private readonly FilePosition position;
        private readonly IList<StepBuilder> steps = new List<StepBuilder>();

        public BackgroundBuilder(string name, string description, FilePosition position)
        {
            this.text = TextHelper.GetText(name, description);
            this.position = position;
        }

        public void ProcessStep(StepBuilder step)
        {
            steps.Add(step);
        }

        public Background GetResult()
        {
            return new Background(text, new ScenarioSteps(steps.Select(s => s.GetResult()).ToArray())) { FilePosition = position };
        }
    }
}