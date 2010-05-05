using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class Then : ScenarioStep
    {
    }

    public class Thens : List<Then>
    {
        public Thens()
        {
        }

        public Thens(params Then[] thens)
        {
            AddRange(thens);
        }
    }
}