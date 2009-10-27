using System;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class And : ScenarioStep
    {
        public And()
        {
        }

        public And(Text stepText, MultilineText multilineTextArgument, Table tableArg) : base(stepText, multilineTextArgument, tableArg)
        {
        }
    }
}