using System;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class But : ScenarioStep
    {
        public But()
        {
        }

        public But(Text stepText, MultilineText multilineTextArgument, Table tableArg) : base(stepText, multilineTextArgument, tableArg)
        {
        }
    }
}