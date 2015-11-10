using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gherkin;

namespace TechTalk.SpecFlow.Parser
{
    public static class ParserExceptionExtensions
    {
        public static ParserException[] GetParserExceptions(this ParserException parserException)
        {
            var composite = parserException as CompositeParserException;
            if (composite != null)
                return composite.Errors.ToArray();

            return new[] {parserException};
        }
    }
}
