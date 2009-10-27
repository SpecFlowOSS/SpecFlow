using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class Given : ScenarioStep
    {
        public Given()
        {
        }

        public Given(Text givenText, MultilineText multilineTextArgument, Table tableArg) : base(givenText, multilineTextArgument, tableArg)
        {
        }
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
    