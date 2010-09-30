using TechTalk.SpecFlow.Parser.Gherkin;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class GherkinSemanticErrorException : ScanningErrorException
    {
        public GherkinSemanticErrorException(string message, FilePosition position)
            : base(message, position.Line - 1, position.Column - 1)
        {
        }
    }
}