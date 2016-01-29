using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class When : ScenarioStep
    {
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