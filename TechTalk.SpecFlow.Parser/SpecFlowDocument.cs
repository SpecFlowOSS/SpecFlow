using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gherkin.Ast;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowDocument : GherkinDocument
    {
        public SpecFlowDocument(SpecFlowFeature feature, Comment[] comments) : base(feature, comments)
        {
        }

        public SpecFlowFeature SpecFlowFeature => (SpecFlowFeature) Feature;
    }
}
