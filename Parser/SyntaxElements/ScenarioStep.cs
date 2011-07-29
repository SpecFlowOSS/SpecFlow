using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TechTalk.SpecFlow.Parser.Gherkin;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    [XmlInclude(typeof(Given))]
    [XmlInclude(typeof(When))]
    [XmlInclude(typeof(Then))]
    [XmlInclude(typeof(And))]
    [XmlInclude(typeof(But))]
    public class ScenarioStep
    {
        public ScenarioBlock ScenarioBlock { get; set; }
        public StepKeyword StepKeyword { get; set; }
        public string Keyword { get; set; }
        public string Text { get; set; }
        public string MultiLineTextArgument { get; set; }
        public GherkinTable TableArg { get; set; }
        public FilePosition FilePosition { get; set; }

        public ScenarioStep Clone()
        {
            return (ScenarioStep)MemberwiseClone();
        }
    }

    public class ScenarioSteps : List<ScenarioStep>
    {
        public ScenarioSteps()
        {
        }

        public ScenarioSteps(params ScenarioStep[] thens)
        {
            AddRange(thens);
        }
    }
}