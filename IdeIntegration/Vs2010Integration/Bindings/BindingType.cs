using System;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.Vs2010Integration.Bindings
{
    public enum BindingType
    {
        Given = ScenarioBlock.Given,
        When = ScenarioBlock.When,
        Then = ScenarioBlock.Then
    }
}
