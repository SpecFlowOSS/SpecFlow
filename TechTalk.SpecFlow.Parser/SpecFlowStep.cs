using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gherkin;
using Gherkin.Ast;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowStep : Step
    {
        public ScenarioBlock ScenarioBlock { get; private set; }
        public StepKeyword StepKeyword { get; private set; }

        public SpecFlowStep(Location location, string keyword, StepKeywordType keywordType, string text, StepArgument argument, StepKeyword stepKeyword, ScenarioBlock scenarioBlock) : base(location, keyword, keywordType, text, argument)
        {
            StepKeyword = stepKeyword;
            ScenarioBlock = scenarioBlock;
        }
    }
}
