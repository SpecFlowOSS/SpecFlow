using Gherkin;
using System.Globalization;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowGherkinParserFactory : IGherkinParserFactory
    {
        public IGherkinParser Create(IGherkinDialectProvider dialectProvider) => new SpecFlowGherkinParser(dialectProvider);

        public IGherkinParser Create(CultureInfo cultureInfo) => new SpecFlowGherkinParser(cultureInfo);
    }
}