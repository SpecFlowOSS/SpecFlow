using System;
using System.Linq;

namespace TechTalk.SpecFlow
{
    public enum ScenarioBlock
    {
        None,
        Given,
        When,
        Then,
    }

    internal enum StepDefinitionKeyword
    {
        Given = ScenarioBlock.Given,
        When = ScenarioBlock.When,
        Then = ScenarioBlock.Then,
        And,
        But
    }
}