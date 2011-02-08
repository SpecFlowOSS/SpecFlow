using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class ScenarioBuilder : IScenarioBuilder
    {
        protected readonly string keyword;
        protected readonly string title;
        protected readonly string description;
        protected readonly FilePosition position;
        protected readonly Tags tags;
        protected readonly IList<StepBuilder> steps = new List<StepBuilder>();

        public string Title
        {
            get { return title; }
        }

        public FilePosition Position
        {
            get { return position; }
        }

        public ScenarioBuilder(string keyword, string name, string description, Tags tags, FilePosition position)
        {
            this.keyword = keyword;
            this.title = name;
            this.description = description;
            this.position = position;
            this.tags = tags;
        }

        public void ProcessStep(StepBuilder step)
        {
            steps.Add(step);
        }

        public virtual Scenario GetResult()
        {
            return new Scenario(keyword, title, description, tags, new ScenarioSteps(steps.Select(s => s.GetResult()).ToArray())) { FilePosition = position };
        }
    }
}