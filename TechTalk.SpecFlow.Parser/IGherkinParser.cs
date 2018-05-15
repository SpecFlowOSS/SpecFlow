using Gherkin;
using System.IO;

namespace TechTalk.SpecFlow.Parser
{
    public interface IGherkinParser
    {
        SpecFlowDocument Parse(TextReader featureFileReader, string sourceFilePath);

        IGherkinDialectProvider DialectProvider { get; }
    }
}