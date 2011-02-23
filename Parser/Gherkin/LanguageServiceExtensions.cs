using System;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    public static class LanguageServiceExtensions
    {
        public static ScenarioBlock? ToScenarioBlock(this StepKeyword stepKeyword)
        {
            switch (stepKeyword)
            {
                case StepKeyword.Given:
                    return ScenarioBlock.Given;
                case StepKeyword.When:
                    return ScenarioBlock.When;
                case StepKeyword.Then:
                    return ScenarioBlock.Then;
            }
            return null;
        }
    }
}
