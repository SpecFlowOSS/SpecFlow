using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Gherkin;
using Gherkin.Ast;
using Gherkin3Parser=Gherkin.Parser;

namespace TechTalk.SpecFlow.Parser
{
    public class SpecFlowGherkinParser
    {
        private readonly GherkinDialectProvider dialectProvider;

        internal GherkinDialectProvider DialectProvider
        {
            get { return dialectProvider; }
        }

        public SpecFlowGherkinParser(CultureInfo defaultLanguage)
        {
            this.dialectProvider = new GherkinDialectProvider("en"); //TODO[Gherkin3] downgrade defaultLanguage
        }

        public Feature Parse(TextReader featureFileReader, string sourceFilePath)
        {
            var parser = new Gherkin3Parser();
            var tokenMatcher = new TokenMatcher(dialectProvider);
            var feature = parser.Parse(new TokenScanner(featureFileReader), tokenMatcher, new AstBuilder<Feature>());
            //TODO[Gherkin3]: add source file path
            return feature;
        }
    }
}
