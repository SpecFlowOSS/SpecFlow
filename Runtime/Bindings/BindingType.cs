using System;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings
{
    public enum BindingType
    {
        Given = ScenarioBlock.Given,
        When = ScenarioBlock.When,
        Then = ScenarioBlock.Then
    }

    internal static class BindingTypeHelper
    {
        public static BindingType ToBindingType(this ScenarioBlock block)
        {
            if (block != ScenarioBlock.Given && 
                block != ScenarioBlock.When &&
                block != ScenarioBlock.Then)
                throw new ArgumentException("Unable to convert block to binding type", "block");

            return (BindingType)((int)block);
        }

        public static ScenarioBlock ToScenarioBlock(this BindingType bindingType)
        {
            return (ScenarioBlock)((int)bindingType);
        }

        public static StepDefinitionKeyword ToStepDefinitionKeyword(this BindingType bindingType)
        {
            return (StepDefinitionKeyword)((int)bindingType);
        }

        public static bool Equals(this ScenarioBlock block, BindingType bindingType)
        {
            return (int)block == (int)bindingType;
        }
    }
}