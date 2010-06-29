using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class FeatureBuilder
    {
        private string title;
        private string description;
        private string sourceFilePath;
        private Tags tags;
        private readonly IList<IScenarioBuilder> scenarios = new List<IScenarioBuilder>();
        private BackgroundBuilder background = null;

        public string SourceFilePath
        {
            get { return sourceFilePath; }
            set { sourceFilePath = value; }
        }

        public void SetHeader(string title, string description, Tags tags)
        {
            this.title = title;
            this.description = description;
            this.tags = tags;
        }

        public Feature GetResult()
        {
            var feature = new Feature(
                title,
                tags,
                description,
                background == null ? null : background.GetResult(),
                scenarios.Select(sb => sb.GetResult()).ToArray());
            feature.SourceFile = sourceFilePath;
            return feature;
        }

        public void AddScenario(IScenarioBuilder scenarioBuilder)
        {
            scenarios.Add(scenarioBuilder);
        }

        public void AddBackground(BackgroundBuilder backgroundBuilder)
        {
            background = backgroundBuilder;
        }
    }
}