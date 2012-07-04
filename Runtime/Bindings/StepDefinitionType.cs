using System;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings
{
    public enum StepDefinitionType
    {
        Given = ScenarioBlock.Given,
        When = ScenarioBlock.When,
        Then = ScenarioBlock.Then
    }

    internal static class BindingTypeHelper
    {
        public static StepDefinitionType ToBindingType(this ScenarioBlock block)
        {
            if (block != ScenarioBlock.Given && 
                block != ScenarioBlock.When &&
                block != ScenarioBlock.Then)
                throw new ArgumentException("Unable to convert block to binding type", "block");

            return (StepDefinitionType)((int)block);
        }

        public static ScenarioBlock ToScenarioBlock(this StepDefinitionType stepDefinitionType)
        {
            return (ScenarioBlock)((int)stepDefinitionType);
        }

        public static StepDefinitionKeyword ToStepDefinitionKeyword(this StepDefinitionType stepDefinitionType)
        {
            return (StepDefinitionKeyword)((int)stepDefinitionType);
        }

        public static bool Equals(this ScenarioBlock block, StepDefinitionType stepDefinitionType)
        {
            return (int)block == (int)stepDefinitionType;
        }
    }
}