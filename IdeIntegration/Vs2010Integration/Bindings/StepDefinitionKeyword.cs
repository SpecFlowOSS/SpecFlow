using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.Bindings
{
    public enum StepDefinitionKeyword
    {
        Given = StepKeyword.Given,
        When = StepKeyword.When,
        Then = StepKeyword.Then,
        And = StepKeyword.And,
        But = StepKeyword.But
    }
}