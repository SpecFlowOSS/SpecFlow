using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gherkin.Ast;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowFeature : Feature
    {
        public string SourceFilePath { get; private set; }
        public SpecFlowFeature(Tag[] tags, Location location, string language, string keyword, string name, string description, Background background, ScenarioDefinition[] scenarioDefinitions, Comment[] comments, string sourceFilePath) : base(tags, location, language, keyword, name, description, background, scenarioDefinitions, comments)
        {
            this.SourceFilePath = sourceFilePath;
        }
    }
}
