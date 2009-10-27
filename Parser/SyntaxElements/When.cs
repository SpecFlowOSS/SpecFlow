using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class When : ScenarioStep
    {
        public When()
        {
        }

        public When(Text givenText, MultilineText multilineTextArgument, Table tableArg) : base(givenText, multilineTextArgument, tableArg)
        {
        }
    }

    public class Whens : List<When>
    {
        public Whens()
        {
        }

        public Whens(params When[] whens)
        {
            AddRange(whens);
        }
    }
}