using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gherkin.Ast;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowScenarioOutline : ScenarioOutline
    {
        public SpecFlowScenarioOutline(Tag[] tags, Location location, string keyword, string name, string description, Step[] steps, Examples[] examples) : 
            base(tags, location, keyword, name, description, steps, examples)
        {
        }
    }
}
