using Gherkin;
using Gherkin.Ast;

namespace TechTalk.SpecFlow.Parser
{
    public class SemanticParserException : ParserException
    {
        public SemanticParserException(string message) : base(message)
        {
        }

        public SemanticParserException(string message, Location location) : base(message, location)
        {
        }
    }
}