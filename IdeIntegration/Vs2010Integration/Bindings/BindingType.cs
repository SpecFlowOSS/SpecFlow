using System;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.Bindings
{
    public enum BindingType
    {
        Given = ScenarioBlock.Given,
        When = ScenarioBlock.When,
        Then = ScenarioBlock.Then
    }
}
