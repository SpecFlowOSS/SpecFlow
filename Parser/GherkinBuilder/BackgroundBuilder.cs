using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class BackgroundBuilder : IStepProcessor
    {
        private readonly string keyword;
        private readonly string name;
        private readonly string description;
        private readonly FilePosition position;
        private readonly IList<StepBuilder> steps = new List<StepBuilder>();

        public FilePosition Position
        {
            get { return position; }
        }

        public BackgroundBuilder(string keyword, string name, string description, FilePosition position)
        {
            this.keyword = keyword;
            this.name = name;
            this.description = description;
            this.position = position;
        }

        public void ProcessStep(StepBuilder step)
        {
            steps.Add(step);
        }

        public Background GetResult()
        {
            return new Background(keyword, name, description, new ScenarioSteps(steps.Select(s => s.GetResult()).ToArray())) { FilePosition = position };
        }
    }
}