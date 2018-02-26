using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Gherkin.Ast;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class ReportStep
    {
        public ReportStep()
        {
        }

        public ReportStep(Location location, string keyword, string text, ReportStepArgument argument, StepKeyword? stepKeyword, Parser.ScenarioBlock? scenarioBlock)
        {
            //Location = location;
            Keyword = keyword;
            Text = text;
            Argument = argument;
            ScenarioBlock = scenarioBlock;
            StepKeyword = stepKeyword;
        }

        //public Location Location { get; set; }

        public string Keyword { get; set; }

        public string Text { get; set; }

        public ReportStepArgument Argument { get; set; }
        public TechTalk.SpecFlow.Parser.ScenarioBlock? ScenarioBlock { get; set; }
        public Parser.StepKeyword? StepKeyword { get; set; }
    }
}
