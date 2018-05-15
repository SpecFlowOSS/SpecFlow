using Gherkin;
using System.Globalization;

namespace TechTalk.SpecFlow.Parser
{
    public interface IGherkinParserFactory
    {
        IGherkinParser Create(IGherkinDialectProvider dialectProvider);
        IGherkinParser Create(CultureInfo cultureInfo);
    }
}
