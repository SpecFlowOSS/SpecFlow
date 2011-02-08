using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class FeatureBuilder
    {
        private string keyword;
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

        public void SetHeader(string keyword, string title, string description, Tags tags)
        {
            this.keyword = keyword;
            this.title = title;
            this.description = description;
            this.tags = tags;
        }

        public Feature GetResult()
        {
            var scenarioResults = scenarios.Select(sb => sb.GetResult()).ToArray();

            var feature = new Feature(
                keyword,
                title,
                tags,
                description,
                background == null ? null : background.GetResult(),
                scenarioResults);
            feature.SourceFile = sourceFilePath;
            return feature;
        }

        public void AddScenario(IScenarioBuilder scenarioBuilder)
        {
            if (scenarios.Any(s => s.Title.Equals(scenarioBuilder.Title)))
                throw new GherkinSemanticErrorException(
                    string.Format("Feature file already contains a scenario with name '{0}'", scenarioBuilder.Title),
                    scenarioBuilder.Position);

            scenarios.Add(scenarioBuilder);
        }

        public void AddBackground(BackgroundBuilder backgroundBuilder)
        {
            if (background != null)
                throw new GherkinSemanticErrorException(
                    "Feature file already contains a background section.",
                    backgroundBuilder.Position);
            background = backgroundBuilder;
        }
    }
}