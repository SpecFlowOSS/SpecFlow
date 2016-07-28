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

        public SpecFlowFeature(Tag[] tags, Location location, string language, string keyword, string name, string description, ScenarioDefinition[] children)
            : base(tags, location, language, keyword, name, description, children)
        {

            if (Children != null)
            {
                ScenarioDefinitions = Children.Where(child => !(child is Background)).ToList();

                var background = Children.SingleOrDefault(child => child is Background);

                if (background != null)
                {
                    Background = (Background)background;
                }
            }

        }

        public IEnumerable<ScenarioDefinition> ScenarioDefinitions { get; private set; }

        public Background Background
        {
            get; private set;
        }
    }
}
