using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    [XmlInclude(typeof(Given))]
    [XmlInclude(typeof(When))]
    [XmlInclude(typeof(Then))]
    [XmlInclude(typeof(And))]
    [XmlInclude(typeof(But))]
    public class ScenarioStep
    {
        public string Text { get; set; }
        public string MultiLineTextArgument { get; set; }
        public Table TableArg { get; set; }
        public int? SourceFileLine { get; set; }

        public ScenarioStep()
        {
        }

        public ScenarioStep(Text stepText, MultilineText multilineTextArgument, Table tableArg)
        {
            this.Text = stepText.Value;
            MultiLineTextArgument = multilineTextArgument == null ? null : multilineTextArgument.Value;
            this.TableArg = tableArg;
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