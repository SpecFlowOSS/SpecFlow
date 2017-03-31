using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Gherkin.Ast;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    
    public class TableArgument : ReportStepArgument
    {
        public TableArgument()
        {
            
        }

        public TableArgument(List<ReportTableRow> rows)
        {
            Rows = rows;
        }

        public List<ReportTableRow> Rows { get; private set; }
    }
}
