using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class Given : ScenarioStep
    {
    }

    public class Givens : List<Given>
    {
        public Givens()
        {
        }

        public Givens(params Given[] givens)
        {
            AddRange(givens);
        }
    }
}
    