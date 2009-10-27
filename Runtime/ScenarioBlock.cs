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
        Given,
        When,
        Then,
        And,
        But
    }
}